import api from "../api.ts";
import type { SignatureWithRecovery } from "@noble/secp256k1";

export interface LoginRequestValues {
  username: string;
}

export interface EncryptedKey {
  publicKey: string;
  encryptedPrivateKey: string;
  iv: string;
  salt: string;
}

export interface RegisterValues {
  username: string;
  email: string;
  fullName: string;
  encryptedKey: EncryptedKey;
}

export const login = async ({ username }: LoginRequestValues) => {
  const response = await api.post("/auth/login", {
    username,
  });
  return response.data;
};

interface SignInValues {
  username: string;
  nonce: string;
  signature: SignatureWithRecovery;
}

export interface SignResponse {
  nonce: string;
  encryptedKey: EncryptedKey;
}

export interface EccSignature {
  r: string;
  s: string;
}

export const sign = async ({ username, nonce, signature }: SignInValues) => {
  const response = await api.post("/auth/sign", {
    username,
    nonce,
    signature: SignatureWithRecoveryToStrings(signature),
  });
  return response.data;
};

export const SignatureWithRecoveryToStrings = (
  signatureWithRecovery: SignatureWithRecovery,
) => {
  return {
    r: signatureWithRecovery.r.toString(),
    s: signatureWithRecovery.s.toString(),
  };
};

export const registerUser = async ({
  username,
  email,
  fullName,
  encryptedKey,
}: RegisterValues) => {
  const response = await api.post("/auth/register", {
    username,
    email,
    fullName,
    encryptedKey,
  });
  return response.data;
};

export const getProfile = async () => {
  const response = await api.get("/users/me");
  return response.data;
};
