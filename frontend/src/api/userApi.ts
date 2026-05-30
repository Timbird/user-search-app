import type { CreateUserRequest, User } from '../types/user';

const BASE = '/api/users';

export async function autocomplete(q: string): Promise<string[]> {
  const res = await fetch(`${BASE}/autocomplete?q=${encodeURIComponent(q)}`);
  if (!res.ok) return [];
  return res.json();
}

export async function search(q: string): Promise<User[]> {
  const res = await fetch(`${BASE}/search?q=${encodeURIComponent(q)}`);
  if (!res.ok) return [];
  return res.json();
}

export async function createUser(
  data: CreateUserRequest
): Promise<{ user?: User; error?: string; validationErrors?: Record<string, string[]> }> {
  const res = await fetch(BASE, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });

  if (res.status === 201) return { user: await res.json() };
  if (res.status === 409) {
    const body = await res.json();
    return { error: body.message };
  }
  if (res.status === 400) {
    const body = await res.json();
    return { validationErrors: body.errors };
  }
  return { error: 'An unexpected error occurred.' };
}
