import { useState } from 'react';
import { createUser } from '../api/userApi';
import type { CreateUserRequest, User } from '../types/user';
import { isValidEmail, isValidPhone } from '../utils/validation';

interface FormErrors {
  firstName?: string;
  lastName?: string;
  jobTitle?: string;
  phone?: string;
  email?: string;
}

const empty: CreateUserRequest = { firstName: '', lastName: '', jobTitle: '', phone: '', email: '' };

export function useCreateUser(onSuccess: (user: User) => void) {
  const [form, setForm] = useState<CreateUserRequest>(empty);
  const [errors, setErrors] = useState<FormErrors>({});
  const [submitting, setSubmitting] = useState(false);
  const [serverError, setServerError] = useState<string | null>(null);

  const validate = (): boolean => {
    const e: FormErrors = {};
    if (!form.firstName.trim()) e.firstName = 'First name is required.';
    if (!form.lastName.trim()) e.lastName = 'Last name is required.';
    if (!form.jobTitle.trim()) e.jobTitle = 'Job title is required.';
    if (!form.phone.trim()) e.phone = 'Phone is required.';
    else if (!isValidPhone(form.phone)) e.phone = 'Enter a valid UK mobile number (e.g. 07789 543768).';
    if (!form.email.trim()) e.email = 'Email is required.';
    else if (!isValidEmail(form.email)) e.email = 'Enter a valid email address.';
    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const handleChange = (field: keyof CreateUserRequest, value: string) => {
    setForm(f => ({ ...f, [field]: value }));
    setErrors(e => ({ ...e, [field]: undefined }));
    setServerError(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    setSubmitting(true);
    const result = await createUser(form);
    setSubmitting(false);
    if (result.user) {
      setForm(empty);
      setErrors({});
      onSuccess(result.user);
    } else {
      setServerError(result.error ?? 'Something went wrong.');
    }
  };

  return { form, errors, submitting, serverError, handleChange, handleSubmit };
}
