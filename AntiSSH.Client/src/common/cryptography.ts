import { utils, getPublicKey, signAsync } from "@noble/secp256k1";
import type { EncryptedKey } from "./fetches.ts";

async function deriveKey(passphrase: Uint8Array, salt: Uint8Array) {
  const keyMaterial = await crypto.subtle.importKey(
    "raw",
    passphrase,
    "PBKDF2",
    false,
    ["deriveKey"],
  );

  return await crypto.subtle.deriveKey(
    {
      name: "PBKDF2",
      salt,
      iterations: 100_000,
      hash: "SHA-256",
    },
    keyMaterial,
    { name: "AES-CBC", length: 256 },
    false,
    ["encrypt", "decrypt"],
  );
}

export async function encryptPrivateKey(
  password: string,
): Promise<EncryptedKey> {
  const passphrase = new TextEncoder().encode(password);
  const privateKey = utils.randomPrivateKey();

  const salt = crypto.getRandomValues(new Uint8Array(16));
  const iv = crypto.getRandomValues(new Uint8Array(16));

  const aesKey = await deriveKey(passphrase, salt);
  const encryptedKey = await crypto.subtle.encrypt(
    { name: "AES-CBC", iv },
    aesKey,
    privateKey,
  );

  return {
    publicKey: Base64Url(getPublicKey(privateKey)),
    encryptedPrivateKey: Base64Url(new Uint8Array(encryptedKey)),
    salt: Base64Url(salt),
    iv: Base64Url(iv),
  };
}

export function Base64UrlDecode(base64: string): Uint8Array {
  base64 = base64.replace(/-/g, "+").replace(/_/g, "/");
  switch (base64.length % 4) {
    case 2:
      base64 += "==";
      break;
    case 3:
      base64 += "=";
      break;
  }

  const binary = atob(base64);
  const bytes = new Uint8Array(binary.length);
  for (let i = 0; i < binary.length; i++) {
    bytes[i] = binary.charCodeAt(i);
  }
  return bytes;
}

export function Base64Url(bytes: Uint8Array): string {
  const binary = String.fromCharCode(...bytes);
  return btoa(binary);
}

export async function decryptPrivateKey(
  encryptedKey: Uint8Array,
  password: string,
  salt: Uint8Array,
  iv: Uint8Array,
) {
  const passphrase = new TextEncoder().encode(password);
  const aesKey = await deriveKey(passphrase, salt);
  const dec = await crypto.subtle.decrypt(
    { name: "AES-CBC", iv },
    aesKey,
    encryptedKey,
  );
  return new Uint8Array(dec);
}

export async function signNonce(privateKey: Uint8Array, nonce: string) {
  return await signAsync(Base64UrlDecode(nonce), privateKey);
}
