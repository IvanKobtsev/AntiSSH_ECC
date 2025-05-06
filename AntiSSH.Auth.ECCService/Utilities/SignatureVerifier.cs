using AntiSSH.Auth.ECC.DTOs;
using NSec.Cryptography;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;

namespace AntiSSH.Auth.ECC.Utilities;

public static class SignatureVerifier
{
    public static bool VerifySignature(
        byte[] publicKeyBytes,
        byte[] message,
        EccSignatureDto signature
    )
    {
        var curve = SecNamedCurves.GetByName("secp256k1");
        var ecParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

        // Decode compressed public key
        var q = curve.Curve.DecodePoint(publicKeyBytes);
        var publicKeyParams = new ECPublicKeyParameters(q, ecParams);

        // Parse r and s from 64-byte signature (r || s)
        var r = new BigInteger(signature.R);
        var s = new BigInteger(signature.S);

        var signer = new ECDsaSigner();
        signer.Init(false, publicKeyParams);

        return signer.VerifySignature(message, r, s);
    }
}
