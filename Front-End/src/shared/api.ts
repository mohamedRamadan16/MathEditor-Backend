export const API_URL =
  process.env.NEXT_PUBLIC_API_URL || "http://localhost:5041";

export function apiFetch(path: string, options?: RequestInit) {
  // Ensure path starts with '/api/'
  const normalizedPath = path.startsWith("/api/")
    ? path
    : `/api/${path.replace(/^\//, "")}`;
  return fetch(`${API_URL}${normalizedPath}`, options);
}
