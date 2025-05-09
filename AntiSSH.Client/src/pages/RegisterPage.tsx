import { useMutation } from "@tanstack/react-query";
import { registerUser } from "../common/fetches.ts";
import { encryptPrivateKey } from "../common/cryptography.ts";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import styles from "../styles/main.module.scss";

interface RegisterFormValues {
  username: string;
  password: string;
  fullName: string;
  email: string;
}

export default function RegisterPage() {
  const { register, handleSubmit } = useForm<RegisterFormValues>();
  const navigate = useNavigate();

  const registerMutation = useMutation({
    mutationFn: registerUser,
    onSuccess: (_) => {
      navigate("/login");
    },
    onError: () => {
      alert("Invalid registering credentials!");
    },
  });

  const onSubmit = async (values: RegisterFormValues) => {
    const keyDto = await encryptPrivateKey(values.password);

    const requestBody = {
      ...values,
      encryptedKey: keyDto,
    };

    console.log(JSON.stringify(requestBody));

    registerMutation.mutate(requestBody);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className={styles.container}>
      <h1>Register</h1>
      <input
        {...register("username")}
        type="text"
        name="username"
        placeholder="Username"
      />
      <input
        {...register("fullName")}
        type="text"
        name="fullName"
        placeholder="Full name"
      />
      <input
        {...register("email")}
        type="text"
        name="email"
        placeholder="Email"
      />
      <input
        {...register("password")}
        type="text"
        name="password"
        placeholder="Password"
      />
      <button type="submit">Register</button>
    </form>
  );
}
