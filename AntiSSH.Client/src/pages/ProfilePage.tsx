import { useQuery } from "@tanstack/react-query";
import { getProfile } from "../common/fetches.ts";
import styles from "../styles/main.module.scss";

export default function ProfilePage() {
  const { data, isLoading, error } = useQuery({
    queryKey: ["profile"],
    queryFn: getProfile,
  });

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error.message}</div>;
  }

  return (
    <div className={styles.container}>
      <h1>Profile</h1>
      <h5>Username: {data.username}</h5>
      <h5>Full name: {data.fullname}</h5>
      <h5>Email: {data.email}</h5>
    </div>
  );
}
