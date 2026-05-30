import type { User } from '../../types/user';
import { useCreateUser } from '../../hooks/useCreateUser';
import './NewUserForm.scss';

interface Props {
  onSuccess: (user: User) => void;
  onClose: () => void;
}

export function NewUserForm({ onSuccess, onClose }: Props) {
  const { form, errors, submitting, serverError, handleChange, handleSubmit, reset } =
    useCreateUser(user => {
      onSuccess(user);
      onClose();
    });

  const fields: { label: string; key: keyof typeof form; type?: string; placeholder?: string }[] = [
    { label: 'First name', key: 'firstName', placeholder: 'First name' },
    { label: 'Last name', key: 'lastName', placeholder: 'Last name' },
    { label: 'Job title', key: 'jobTitle', placeholder: 'Job title' },
    { label: 'Phone', key: 'phone', placeholder: 'e.g. 07789 543768' },
    { label: 'Email', key: 'email', type: 'email', placeholder: 'Email' },
  ];

  return (
    <div className="new-user-form">
      <form onSubmit={handleSubmit} noValidate>
        <div className="new-user-form__fields">
          {fields.map(({ label, key, type, placeholder }) => (
            <div className="new-user-form__field" key={key}>
              <label className="new-user-form__label">{label}</label>
              <input
                className={`new-user-form__input${errors[key] ? ' new-user-form__input--error' : ''}`}
                type={type ?? 'text'}
                placeholder={placeholder}
                value={form[key]}
                onChange={e => handleChange(key, e.target.value)}
              />
              {errors[key] && <span className="new-user-form__error">{errors[key]}</span>}
            </div>
          ))}
        </div>

        {serverError && <p className="new-user-form__server-error">{serverError}</p>}

        <div className="new-user-form__actions">
          <button type="submit" className="new-user-form__btn new-user-form__btn--primary" disabled={submitting}>
            {submitting ? 'Creating...' : 'Create'}
          </button>
          <button type="button" className="new-user-form__btn new-user-form__btn--secondary" onClick={() => { reset(); onClose(); }}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}
