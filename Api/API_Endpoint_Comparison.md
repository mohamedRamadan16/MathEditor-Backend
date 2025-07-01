# API Endpoint Comparison: Old Next.js vs New .NET Backend

## 1. Users

| Old Next.js Endpoint   | New .NET Endpoint               |
| ---------------------- | ------------------------------- |
| GET /api/users/[id]    | GET /api/users/{handleOrId}     |
| PATCH /api/users/[id]  | PUT /api/users/{id}             |
| DELETE /api/users/[id] | DELETE /api/users/{id}          |
| GET /api/users/check   | (missing, or handled elsewhere) |
| (not in old)           | GET /api/users/by-email/{email} |

---

## 2. Documents

| Old Next.js Endpoint                              | New .NET Endpoint                     |
| ------------------------------------------------- | ------------------------------------- |
| GET /api/documents                                | GET /api/documents                    |
| POST /api/documents                               | POST /api/documents                   |
| GET /api/documents/[id]                           | GET /api/documents/{id}               |
| PATCH /api/documents/[id]                         | PUT /api/documents                    |
| DELETE /api/documents/[id]                        | DELETE /api/documents/{id}            |
| GET /api/documents/check                          | (missing, or handled elsewhere)       |
| GET /api/documents/new/[id]                       | POST /api/documents/new/{id}          |
| GET /api/documents/by-handle/{handle}             | GET /api/documents/by-handle/{handle} |
| (not in old) PUT /api/documents/{id}/head         | PUT /api/documents/{id}/head          |
| (not in old) POST /api/documents/{id}/coauthors   | POST /api/documents/{id}/coauthors    |
| (not in old) DELETE /api/documents/{id}/coauthors | DELETE /api/documents/{id}/coauthors  |

---

## 3. Revisions

| Old Next.js Endpoint       | New .NET Endpoint          |
| -------------------------- | -------------------------- |
| GET /api/revisions/[id]    | GET /api/revisions/{id}    |
| POST /api/revisions        | POST /api/revisions        |
| DELETE /api/revisions/[id] | DELETE /api/revisions/{id} |

---

## 4. Auth

| Old Next.js Endpoint | New .NET Endpoint       |
| -------------------- | ----------------------- |
| (NextAuth.js routes) | POST /api/auth/login    |
|                      | POST /api/auth/register |
|                      | POST /api/auth/logout   |
|                      | GET /api/auth/session   |

---

## 5. Miscellaneous / Utility

| Old Next.js Endpoint     | New .NET Endpoint         |
| ------------------------ | ------------------------- |
| GET /api/usage           | (missing)                 |
| GET /api/revalidate      | (missing)                 |
| GET /api/og              | (missing)                 |
| GET /api/pdf/[url]       | (missing)                 |
| GET /api/docx/[id]       | (missing)                 |
| POST /api/embed          | POST /embed/html          |
| GET /api/thumbnails/[id] | (handled via /embed/html) |
| GET /api/completion      | (missing)                 |

---

## Observations

- Most core CRUD endpoints are present in both projects.
- Some utility endpoints exist in the old project but are not present in the new .NET backend. Implement as needed.
- The new backend has more explicit endpoints for coauthors and document head management.
- Auth endpoints are more explicit in the new backend.
- The `/embed/html` endpoint in the new backend replaces the old `/embed` POST in Next.js.
