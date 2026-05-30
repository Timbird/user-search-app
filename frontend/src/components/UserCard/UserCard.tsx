import type { User } from '../../types/user';
import './UserCard.scss';

interface Props {
  user: User;
}

export function UserCard({ user }: Props) {
  return (
    <div className="user-card">
      <h3 className="user-card__name">{user.firstName} {user.lastName}</h3>
      <p className="user-card__job">{user.jobTitle}</p>
      <p className="user-card__phone">{user.phone}</p>
      <p className="user-card__email">{user.email}</p>
    </div>
  );
}
