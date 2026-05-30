import type { User } from '../../types/user';
import { UserCard } from '../UserCard/UserCard';
import './UserCardList.scss';

interface Props {
  users: User[];
  hasSearched: boolean;
}

export function UserCardList({ users, hasSearched }: Props) {
  if (!hasSearched) return null;

  if (users.length === 0) {
    return <p className="user-card-list__empty">No users found.</p>;
  }

  return (
    <div className="user-card-list">
      {users.map(user => (
        <UserCard key={user.id} user={user} />
      ))}
    </div>
  );
}
