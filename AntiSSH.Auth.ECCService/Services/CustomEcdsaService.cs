using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;

namespace AntiSSH.Auth.ECC.Services;

public class CustomEcdsaService
{
    private readonly EccCurve _curve =
        new(
            BigInteger.Zero,
            new BigInteger(7),
            BigInteger.Parse(
                "0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F",
                NumberStyles.AllowHexSpecifier
            ),
            new EccPoint(
                BigInteger.Parse(
                    "079BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798",
                    NumberStyles.AllowHexSpecifier
                ),
                BigInteger.Parse(
                    "0483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8",
                    NumberStyles.AllowHexSpecifier
                )
            ),
            BigInteger.Parse(
                "0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141",
                NumberStyles.AllowHexSpecifier
            )
        );

    private BigInteger GenerateRandomK()
    {
        var random = new Random();
        var data = new byte[32];
        random.NextBytes(data);
        var k = new BigInteger(data);
        return k % (_curve.N - 1) + 1;
    }

    public (BigInteger, BigInteger) SignMessage(BigInteger privateKey, byte[] data)
    {
        var r = BigInteger.Zero;
        var s = BigInteger.Zero;

        var hash = SHA256.HashData(data);
        var h = new BigInteger(hash, isUnsigned: true);
        do
        {
            var k = GenerateRandomK();

            var point = _curve.Multiply(k, _curve.G);
            if (point == null)
                continue;

            r = point.X % _curve.N;
            s = (h + r * privateKey) * _curve.ModInverse(k, _curve.N) % _curve.N;
        } while (r == BigInteger.Zero || s == BigInteger.Zero);

        return (r, s);
    }

    public bool VerifySignature(BigInteger privateKey, byte[] data, BigInteger r, BigInteger s)
    {
        var hash = SHA256.HashData(data);
        var h = new BigInteger(hash, isUnsigned: true);
        var w = _curve.ModInverse(s, _curve.N);
        var u1 = h * w % _curve.N;
        var u2 = r * w % _curve.N;

        var publicKey = _curve.Multiply(privateKey, _curve.G);
        if (publicKey == null)
            return false;

        var point = _curve.Add(_curve.Multiply(u1, _curve.G), _curve.Multiply(u2, publicKey));

        return point?.X % _curve.N == r;
    }
}

public class EccPoint(BigInteger x, BigInteger y)
{
    public BigInteger X { get; set; } = x;
    public BigInteger Y { get; set; } = y;
}

public class EccCurve(BigInteger a, BigInteger b, BigInteger p, EccPoint g, BigInteger n)
{
    private BigInteger A { get; } = a;
    private BigInteger B { get; } = b;
    private BigInteger P { get; } = p;
    public EccPoint G { get; } = g;
    public BigInteger N { get; } = n;

    public EccPoint? Add(EccPoint? p1, EccPoint? p2)
    {
        if (p1 == null)
            return p2;
        if (p2 == null)
            return p1;

        BigInteger m;
        if (p1.X == p2.X)
        {
            if (p1.Y != p2.Y)
                return null;

            var numerator = 3 * p1.X * p1.X + a;
            var denominator = 2 * p1.Y;
            m = numerator * ModInverse(denominator, P) % P;
        }
        else
        {
            var numerator = p2.Y - p1.Y;
            var denominator = p2.X - p1.X;
            m = numerator * ModInverse(denominator, P) % P;
        }

        var x3 = (BigInteger.Pow(m, 2) - p1.X - p2.X) % P;
        var y3 = (m * (p1.X - x3) - p1.Y) % P;

        x3 = x3 < 0 ? x3 + P : x3;
        y3 = y3 < 0 ? y3 + P : y3;

        return new EccPoint(x3, y3);
    }

    public EccPoint? Multiply(BigInteger k, EccPoint point)
    {
        EccPoint? result = null;
        var addend = point;
        var scalar = k;

        while (scalar > BigInteger.Zero)
        {
            if ((scalar & BigInteger.One) == BigInteger.One)
            {
                result = Add(result, addend);
            }

            addend = Add(addend, addend);
            scalar = scalar >> 1;
        }

        return result;
    }

    public BigInteger ModInverse(BigInteger x, BigInteger m)
    {
        return BigInteger.ModPow(x, m - 2, m);
    }
}
