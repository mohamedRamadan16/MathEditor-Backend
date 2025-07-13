import {
  ApiResponse,
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  DocumentResponseDto,
  DocumentCreateDto,
  DocumentUpdateDto,
  RevisionResponseDto,
  CreateRevisionDto,
  GetAllDocumentsResponse,
  UserDto,
  LexicalStateDto,
} from "@/types/api";
import { apiFetch } from "@/shared/api";
import { SerializedEditorState } from "lexical";

export class MathEditorApiService {
  private static instance: MathEditorApiService;
  private token: string | null = null;

  private constructor() {
    // Load token from localStorage on client side
    if (typeof window !== "undefined") {
      this.token = localStorage.getItem("matheditor_token");
    }
  }

  public static getInstance(): MathEditorApiService {
    if (!MathEditorApiService.instance) {
      MathEditorApiService.instance = new MathEditorApiService();
    }
    return MathEditorApiService.instance;
  }

  private getHeaders(): HeadersInit {
    const headers: HeadersInit = {
      "Content-Type": "application/json",
    };

    if (this.token) {
      headers["Authorization"] = `Bearer ${this.token}`;
    }

    return headers;
  }

  public setToken(token: string | null) {
    this.token = token;
    if (typeof window !== "undefined") {
      if (token) {
        localStorage.setItem("matheditor_token", token);
      } else {
        localStorage.removeItem("matheditor_token");
      }
    }
  }

  public getToken(): string | null {
    return this.token;
  }

  // Auth endpoints
  public async login(
    credentials: LoginRequest
  ): Promise<ApiResponse<AuthResponse>> {
    const response = await apiFetch("/auth/login", {
      method: "POST",
      headers: this.getHeaders(),
      body: JSON.stringify(credentials),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Login failed");
    }

    const data = await response.json();

    // Store token - backend returns { token: "..." } directly
    if (data.token) {
      this.setToken(data.token);
    }

    // Get user info using the session endpoint
    const userResponse = await apiFetch("/auth/session", {
      method: "GET",
      headers: this.getHeaders(),
    });

    if (!userResponse.ok) {
      throw new Error("Failed to get user session");
    }

    const userSession = await userResponse.json();

    // Return in expected format
    return {
      success: true,
      data: {
        token: data.token,
        user: {
          id: userSession.user.id,
          handle: userSession.user.handle,
          name: userSession.user.name,
          email: userSession.user.email,
          image: userSession.user.image,
        },
      },
    };
  }

  public async register(
    userData: RegisterRequest
  ): Promise<ApiResponse<AuthResponse>> {
    const response = await apiFetch("/auth/register", {
      method: "POST",
      headers: this.getHeaders(),
      body: JSON.stringify(userData),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Registration failed");
    }

    const data = await response.json();

    // Store token - backend returns { token: "..." } directly
    if (data.token) {
      this.setToken(data.token);
    }

    // Get user info using the session endpoint
    const userResponse = await apiFetch("/auth/session", {
      method: "GET",
      headers: this.getHeaders(),
    });

    if (!userResponse.ok) {
      throw new Error("Failed to get user session");
    }

    const userSession = await userResponse.json();

    // Return in expected format
    return {
      success: true,
      data: {
        token: data.token,
        user: {
          id: userSession.user.id,
          handle: userSession.user.handle,
          name: userSession.user.name,
          email: userSession.user.email,
          image: userSession.user.image,
        },
      },
    };
  }

  public async getCurrentUser(): Promise<ApiResponse<UserDto | null>> {
    const response = await apiFetch("/auth/session", {
      method: "GET",
      headers: this.getHeaders(),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Failed to get current user");
    }

    const data = await response.json();

    // Return in expected format
    return {
      success: true,
      data: data.user
        ? {
            id: data.user.id,
            handle: data.user.handle,
            name: data.user.name,
            email: data.user.email,
            image: data.user.image,
          }
        : null,
    };
  }

  public logout() {
    this.setToken(null);
  }

  // Document endpoints
  public async getAllDocuments(
    page: number = 1,
    pageSize: number = 10
  ): Promise<ApiResponse<GetAllDocumentsResponse>> {
    const response = await apiFetch(
      `/documents?page=${page}&pageSize=${pageSize}`,
      {
        method: "GET",
        headers: this.getHeaders(),
      }
    );

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Failed to get documents");
    }

    return response.json();
  }

  public async getMyDocuments(
    page: number = 1,
    pageSize: number = 10
  ): Promise<ApiResponse<GetAllDocumentsResponse>> {
    const response = await apiFetch(
      `/documents/my-documents?page=${page}&pageSize=${pageSize}`,
      {
        method: "GET",
        headers: this.getHeaders(),
      }
    );

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Failed to get user documents");
    }

    return response.json();
  }

  public async getDocument(
    id: string
  ): Promise<ApiResponse<DocumentResponseDto>> {
    const response = await apiFetch(`/documents/${id}`, {
      method: "GET",
      headers: this.getHeaders(),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Failed to get document");
    }

    return response.json();
  }

  public async getDocumentByHandle(
    handle: string
  ): Promise<ApiResponse<DocumentResponseDto>> {
    const response = await apiFetch(`/documents/by-handle/${handle}`, {
      method: "GET",
      headers: this.getHeaders(),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Failed to get document");
    }

    return response.json();
  }

  public async createDocument(
    documentData: DocumentCreateDto
  ): Promise<ApiResponse<DocumentResponseDto>> {
    const response = await apiFetch("/documents", {
      method: "POST",
      headers: this.getHeaders(),
      body: JSON.stringify(documentData),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Failed to create document");
    }

    return response.json();
  }

  public async updateDocument(
    documentData: DocumentUpdateDto
  ): Promise<ApiResponse<DocumentResponseDto>> {
    const response = await apiFetch("/documents", {
      method: "PUT",
      headers: this.getHeaders(),
      body: JSON.stringify(documentData),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Failed to update document");
    }

    return response.json();
  }

  public async deleteDocument(id: string): Promise<ApiResponse<void>> {
    const response = await apiFetch(`/documents/${id}`, {
      method: "DELETE",
      headers: this.getHeaders(),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Failed to delete document");
    }

    return response.json();
  }

  public async forkDocument(
    id: string
  ): Promise<ApiResponse<DocumentResponseDto>> {
    const response = await apiFetch(`/documents/new/${id}`, {
      method: "POST",
      headers: this.getHeaders(),
    });

    if (!response.ok) {
      console.error(
        `Fork document failed with status: ${response.status} ${response.statusText}`
      );
      try {
        const error = await response.json();
        console.error("Fork error response:", error);
        throw new Error(
          error.message || `Failed to fork document (${response.status})`
        );
      } catch (parseError) {
        console.error("Could not parse error response:", parseError);
        throw new Error(
          `Failed to fork document (${response.status}: ${response.statusText})`
        );
      }
    }

    return response.json();
  }

  public async toggleDocumentPublished(
    id: string,
    published: boolean
  ): Promise<ApiResponse<DocumentResponseDto>> {
    console.log(
      "API: toggleDocumentPublished called with id:",
      id,
      "published:",
      published
    );

    const response = await apiFetch(`/documents/${id}/published`, {
      method: "PUT",
      headers: this.getHeaders(),
      body: JSON.stringify({ published }),
    });

    if (!response.ok) {
      const error = await response.json();
      console.error("API Error:", error);
      throw new Error(
        error.message || "Failed to toggle document published status"
      );
    }

    const result = await response.json();
    console.log("API Response:", result);
    return result;
  }

  // Revision endpoints
  public async getRevision(
    id: string
  ): Promise<ApiResponse<RevisionResponseDto>> {
    const response = await apiFetch(`/revisions/${id}`, {
      method: "GET",
      headers: this.getHeaders(),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Failed to get revision");
    }

    return response.json();
  }
  public async createRevision(
    revisionData: CreateRevisionDto
  ): Promise<ApiResponse<RevisionResponseDto>> {
    const response = await apiFetch("/revisions", {
      method: "POST",
      headers: this.getHeaders(),
      body: JSON.stringify(revisionData),
    });

    if (!response.ok) {
      let error;
      let errorText = "";
      try {
        errorText = await response.text();
        error = JSON.parse(errorText);
      } catch (e) {
        error = { message: `HTTP ${response.status}: ${response.statusText}` };
      }
      console.error("API Error Details:", {
        status: response.status,
        statusText: response.statusText,
        errorText: errorText,
        parsedError: error,
        requestData: revisionData,
      });
      throw new Error(
        error.message || `Failed to create revision (${response.status})`
      );
    }

    return response.json();
  }

  public async updateRevision(
    id: string,
    revisionData: CreateRevisionDto
  ): Promise<ApiResponse<RevisionResponseDto>> {
    const response = await apiFetch(`/revisions/${id}`, {
      method: "PUT",
      headers: this.getHeaders(),
      body: JSON.stringify(revisionData),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Failed to update revision");
    }

    return response.json();
  }

  public async deleteRevision(id: string): Promise<ApiResponse<void>> {
    const response = await apiFetch(`/revisions/${id}`, {
      method: "DELETE",
      headers: this.getHeaders(),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || "Failed to delete revision");
    }

    return response.json();
  }

  public async getSession(): Promise<ApiResponse<{ user: UserDto | null }>> {
    const response = await apiFetch("/auth/session", {
      method: "GET",
      headers: this.getHeaders(),
    });

    if (!response.ok) {
      return {
        success: false,
        data: { user: null },
        message: "Failed to get session",
      };
    }

    const data = await response.json();

    return {
      success: true,
      data: {
        user: data.user
          ? {
              id: data.user.id,
              handle: data.user.handle,
              name: data.user.name,
              email: data.user.email,
              image: data.user.image,
            }
          : null,
      },
    };
  }

  // Utility methods to convert between frontend and backend formats
  public static convertSerializedEditorStateToLexicalState(
    state: SerializedEditorState
  ): LexicalStateDto {
    if (!state) {
      console.error("State is null or undefined");
      throw new Error("Cannot convert null or undefined state");
    }

    if (!state.root) {
      console.error("State root is null or undefined");
      throw new Error("Cannot convert state with null or undefined root");
    }

    // Helper function to convert a node recursively
    const convertNode = (node: any): any => {
      if (!node) return node;

      const converted: any = {
        children: node.children ? node.children.map(convertNode) : [],
        direction: node.direction || null,
        format: String(node.format ?? ""), // Ensure format is always a string, handle null/undefined
        indent: node.indent || 0,
        type: node.type || "",
        version: node.version || 1,

        // Text node specific properties
        text: node.text || null,
        detail: node.detail || null,
        mode: node.mode || null,
        style: node.style || null,

        // Math node specific properties - handle string value
        value: node.value ? String(node.value) : null,
        id: node.id || null,

        // Image/Sketch/Graph node specific properties
        src: node.src || null,
        altText: node.altText || null,
        width: node.width || null,
        height: node.height || null,
        showCaption: node.showCaption || null,
        caption: node.caption || null,

        // Additional properties that might be used by various node types
        key: node.key || null,
      };

      // Handle special node types
      if (
        node.type === "sketch" ||
        node.type === "image" ||
        node.type === "graph"
      ) {
        // For sketch nodes, if value is an array, convert it to JSON string
        if (node.value && Array.isArray(node.value)) {
          converted.value = JSON.stringify(node.value);
        }

        // Handle caption properly - if it's an object with editorState, process it
        if (node.caption && typeof node.caption === "object") {
          converted.caption = node.caption;
        }
      } else if (node.type === "math") {
        // For math nodes, ensure value is always a string
        if (node.value) {
          converted.value = String(node.value);
        }
      }

      // Copy any additional properties that might exist on special node types
      // but be careful not to copy problematic properties
      const safePropertyNames = [
        "src",
        "altText",
        "width",
        "height",
        "showCaption",
        "value",
        "style",
        "id",
      ];
      Object.keys(node).forEach((key) => {
        if (
          !converted.hasOwnProperty(key) &&
          node[key] !== null &&
          node[key] !== undefined &&
          safePropertyNames.includes(key)
        ) {
          converted[key] = node[key];
        }
      });

      // Remove null values to match DTO structure
      Object.keys(converted).forEach((key) => {
        if (converted[key] === null) {
          delete converted[key];
        }
      });

      return converted;
    };

    const result = {
      root: {
        children: state.root.children
          ? state.root.children.map(convertNode)
          : [],
        direction: state.root.direction || "ltr",
        format: String(state.root.format ?? ""), // Ensure format is always a string, handle null/undefined
        indent: state.root.indent || 0,
        type: state.root.type || "root",
        version: state.root.version || 1,
      },
    };

    return result;
  }

  public static convertLexicalStateToSerializedEditorState(
    lexicalState: LexicalStateDto
  ): SerializedEditorState {
    return {
      root: {
        children: lexicalState.root.children,
        direction: (lexicalState.root.direction as "ltr" | "rtl") || "ltr",
        format: (lexicalState.root.format as any) || "",
        indent: lexicalState.root.indent || 0,
        type: lexicalState.root.type || "root",
        version: lexicalState.root.version || 1,
      },
    };
  }
}

export default MathEditorApiService;
