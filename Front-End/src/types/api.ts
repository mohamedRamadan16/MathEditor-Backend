// API Response Types that match .NET backend DTOs
export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
}

// Authentication Types
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  handle: string;
}

export interface AuthResponse {
  token: string;
  user: UserDto;
}

export interface UserDto {
  id: string;
  handle: string;
  name: string;
  email: string;
  image?: string;
}

// Document Types
export interface DocumentResponseDto {
  id: string;
  handle: string;
  name: string;
  createdAt: string;
  updatedAt: string;
  published: boolean;
  collab: boolean;
  private: boolean;
  head: string;
  baseId?: string;
  author: UserDto;
  coauthors: UserDto[];
  revisions: DocumentRevisionResponseDto[];
}

export interface DocumentCreateDto {
  handle: string;
  name: string;
  createdAt: string;
  updatedAt: string;
  published: boolean;
  collab: boolean;
  private: boolean;
  coauthors: string[];
  initialRevision: InitialRevisionDto;
}

export interface InitialRevisionDto {
  data: LexicalStateDto;
}

export interface DocumentUpdateDto {
  id: string;
  handle?: string;
  name?: string;
  published?: boolean;
  collab?: boolean;
  private?: boolean;
  // Note: coauthors are managed via separate endpoints
}

// Revision Types
export interface RevisionResponseDto {
  id: string;
  documentId: string;
  createdAt: string;
  author: AuthorDto;
  data: string; // JSON string of lexical state
}

export interface CreateRevisionDto {
  documentId: string;
  data: LexicalStateDto;
}

export interface AuthorDto {
  id: string;
  handle?: string;
  name: string;
  image?: string;
}

export interface DocumentRevisionResponseDto {
  id: string;
  documentId: string;
  createdAt: string;
  author: UserDto;
}

// Lexical State Types
export interface LexicalStateDto {
  root: LexicalRootDto;
}

export interface LexicalRootDto {
  children: LexicalNodeDto[];
  direction?: string;
  format: string;
  indent: number;
  type: string;
  version: number;
}

export interface LexicalNodeDto {
  children?: LexicalNodeDto[];
  direction?: string;
  format: string;
  indent: number;
  type: string;
  version: number;
  // Text node specific properties
  text?: string;
  detail?: number;
  mode?: string;
  style?: string;
}

// Utility Types
export interface PaginatedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface GetAllDocumentsResponse {
  items: DocumentResponseDto[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
