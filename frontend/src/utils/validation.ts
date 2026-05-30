// UK mobile: 07XXX XXXXXX (space optional)
export const isValidPhone = (phone: string): boolean =>
  /^07\d{3}\s?\d{6}$/.test(phone.trim());

export const isValidEmail = (email: string): boolean =>
  /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim());
