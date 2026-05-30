import type { User } from '../../types/user';
import { useCreateUser } from '../../hooks/useCreateUser';
import './NewUserForm.scss';

interface Props {
  onSuccess: (user: User) => void;
  onClose: () => void;
}

export function NewUserForm({ onSuccess, onClose }: Props) {
  const { form, errors, submitting, serverError, handleChange, handleSubmit } =
    useCreateUser(user => {
      onSuccess(user);
      onClose();
    });

  const topRow: { key: keyof typeof form; placeholder: string }[] = [
    { key: 'firstName', placeholder: 'First name' },
    { key: 'lastName',  placeholder: 'Last name' },
  ];

  const bottomRow: { key: keyof typeof form; placeholder: string; type?: string }[] = [
    { key: 'jobTitle', placeholder: 'Job title' },
    { key: 'phone',    placeholder: 'e.g. 07789 543768' },
    { key: 'email',    placeholder: 'Email', type: 'email' },
  ];

  const renderField = (key: keyof typeof form, placeholder: string, type = 'text') => (
    <div className="new-user-form__field" key={key}>
      <input
        className={`new-user-form__input${errors[key] ? ' new-user-form__input--error' : ''}`}
        type={type}
        placeholder={placeholder}
        value={form[key]}
        onChange={e => handleChange(key, e.target.value)}
      />
      {errors[key] && <span className="new-user-form__error">{errors[key]}</span>}
    </div>
  );

  return (
    <div className="new-user-form">
<form onSubmit={handleSubmit} noValidate>
        <div className="new-user-form__row">
          {topRow.map(({ key, placeholder }) => renderField(key, placeholder))}
        </div>
        <div className="new-user-form__row">
          {bottomRow.map(({ key, placeholder, type }) => renderField(key, placeholder, type))}
        </div>

        {serverError && <p className="new-user-form__server-error">{serverError}</p>}

        <div className="new-user-form__actions">
          <button type="submit" className="new-user-form__btn new-user-form__btn--primary" disabled={submitting}>
            {submitting ? 'Creating...' : 'Create'}
          </button>
        </div>
      </form>
    </div>
  );
}
