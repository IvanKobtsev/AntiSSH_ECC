import { useForm } from "react-hook-form";
import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { login, sign, type SignResponse } from "../common/fetches.ts";
import {
  Base64UrlDecode,
  decryptPrivateKey,
  signNonce,
} from "../common/cryptography.ts";
import { useRef } from "react";
import styles from "../styles/main.module.scss";

export interface LoginFormValues {
  username: string;
  password: string;
}

export default function LoginPage() {
  const { register, handleSubmit } = useForm<LoginFormValues>();
  const navigate = useNavigate();
  const username = useRef<string>("");
  const password = useRef<string>("");

  const signMutation = useMutation({
    mutationFn: sign,
    onSuccess: (data) => {
      localStorage.setItem("token", data.accessToken);
      navigate("/profile");
    },
    onError: (e) => {
      console.log(e);
      alert("Invalid login credentials!");
    },
  });

  const onSign = async (data: SignResponse) => {
    const privateKey = await decryptPrivateKey(
      Base64UrlDecode(data.encryptedKey.encryptedPrivateKey),
      password.current,
      Base64UrlDecode(data.encryptedKey.salt),
      Base64UrlDecode(data.encryptedKey.iv),
    );

    console.log("Ended");

    signMutation.mutate({
      username: username.current,
      nonce: data.nonce,
      signature: await signNonce(privateKey, data.nonce),
    });
  };

  const loginMutation = useMutation({
    mutationFn: login,
    onSuccess: async (data) => {
      console.log(data);
      await onSign(data);
    },
    onError: (e) => {
      console.log(e);
      alert("Invalid login credentials!");
    },
  });

  const onSubmit = (values: LoginFormValues) => {
    username.current = values.username;
    password.current = values.password;

    loginMutation.mutate(values);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className={styles.container}>
      <h1>Login</h1>
      <input
        {...register("username")}
        type="text"
        name="username"
        placeholder="Username"
      />
      <input
        {...register("password")}
        type="text"
        name="password"
        placeholder="Password"
      />
      <button type="submit">Login</button>
    </form>
  );
}
