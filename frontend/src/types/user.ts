export interface User {
  id: string;
  firstName: string;
  lastName: string;
  jobTitle: string;
  phone: string;
  email: string;
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  jobTitle: string;
  phone: string;
  email: string;
}
