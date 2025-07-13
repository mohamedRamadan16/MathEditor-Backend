# MathEditor - Collaborative Mathematical Document Editor

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/IBastawisi/math-editor/blob/master/LICENSE)
[![demo](https://img.shields.io/badge/live-demo-blue)](https://matheditor.ml/playground)

A powerful collaborative mathematical document editor built with Next.js and .NET, featuring real-time editing, mathematical notation support, advanced document management, and user authentication.
This is a **Porting** from the amazing project : https://github.com/ibastawisi/matheditor

##  Features

###  Core Functionality

- **Rich Text Editing** - Advanced text formatting, copy/paste, code syntax highlighting
- **Mathematical Expressions** - Integrates with [Mathlive](https://cortexjs.io/mathlive) for LaTeX editing with Virtual Keyboard
- **Graphing & Visualization** - Integrates with [Geogebra](https://www.geogebra.org) for mathematical graphs and shapes
- **Hand-drawn Sketches** - Integrates with [Excalidraw](https://excalidraw.com/) for sketches and diagrams
- **Document Management** - Create, edit, save, and organize mathematical documents
- **User Authentication** - Secure registration, login, and user management
- **Real-time Collaboration** - Multiple users can collaborate on documents
- **Document Forking** - Create copies of existing documents for modification
- **Revision History** - Track changes and restore previous versions
- **Access Control** - Private/public documents with fine-grained permissions

### Advanced Editor Features

- **Lexical Framework** - Facebook's extensible rich text editor
- **Mathematical Notation** - Inline and block math expressions with KaTeX rendering
- **Tables & Media** - Insert tables, images, and sticky notes
- **Export Options** - Export to PDF, DOCX, and other formats
- **Coauthor Management** - Add collaborators and manage document permissions
- **Read-only Viewing** - Share documents with view-only access

### Security & Permissions

- **JWT Authentication** - Secure token-based authentication
- **Role-based Access** - Author, coauthor, and viewer permissions
- **Private Documents** - Keep documents private or publish publicly
- **Collaborative Editing** - Enable real-time collaboration per document

## 🛠️ Technology Stack

### Frontend

- **Next.js 15** - React framework with TypeScript
- **Material-UI (MUI)** - Modern React component library
- **Redux Toolkit** - State management
- **Lexical Editor** - Facebook's extensible text editor framework
- **MathLive** - Mathematical expression input
- **KaTeX** - Mathematical notation rendering
- **Next-PWA** - Progressive Web App capabilities

### Backend

- **.NET 9** - Modern web API framework
- **Entity Framework Core** - Object-relational mapping
- **SQL Server** - Database
- **MediatR** - CQRS pattern implementation
- **AutoMapper** - Object-object mapping
- **JWT Authentication** - Secure token-based authentication

##  Installation & Setup

### Prerequisites

- **Node.js** (v18 or higher)
- **.NET 9 SDK**
- **SQL Server** (LocalDB or full instance)

### Frontend Setup

1. **Install dependencies**

   ```bash
   npm install
   ```

2. **Set up environment variables**

   ```bash
   # Create .env.local file
   NEXT_PUBLIC_API_URL=http://localhost:5041/api
   NEXTAUTH_URL=http://localhost:3000
   ```

3. **Start the development server**
   ```bash
   npm run dev
   ```
   Frontend available at `http://localhost:3000`

### Backend Setup

1. **Navigate to API directory**

   ```bash
   cd ../../../BackEnd-DotNet/Api
   ```

2. **Restore packages and setup database**

   ```bash
   dotnet restore
   dotnet ef database update
   ```

3. **Start the API server**
   ```bash
   dotnet run
   ```
   Backend API available at `http://localhost:5041`

## Usage Guide

### Getting Started

1. **Register/Login** - Create an account or sign in
2. **Create Document** - Click "New Document" to start writing
3. **Mathematical Content** - Use the math toolbar for LaTeX expressions
4. **Collaborate** - Add coauthors and enable real-time editing
5. **Share** - Publish documents or share with specific users

### Mathematical Expressions

- Use the integrated MathLive editor for complex equations
- Type LaTeX directly in math blocks
- Support for fractions, integrals, summations, matrices, and more

### Document Management

- **Fork Documents** - Create your own copy of any accessible document
- **Revision History** - Track all changes and restore previous versions
- **Access Control** - Set documents as private, collaborative, or public
- **Export Options** - Download as PDF, DOCX, or other formats

### Collaboration Features

- **Coauthor Management** - Add collaborators by email
- **Real-time Editing** - See changes as others make them
- **Permission Levels** - Author, coauthor, and viewer access
- **Read-only Sharing** - Share documents without edit permissions


## 📋 Available Scripts

### Frontend

```bash
npm run dev          # Development server
npm run build        # Production build
npm run start        # Production server
npm run lint         # Run linting
npm run type-check   # TypeScript validation
```

### Backend

```bash
dotnet run           # Development server
dotnet build         # Build project
dotnet test          # Run tests
dotnet ef migrations add <name>  # Add migration
dotnet ef database update       # Update database
```

## Troubleshooting

### Common Issues

1. **Fork document errors** - Ensure proper authentication and document access
2. **Database connection** - Verify SQL Server is running and connection string is correct
3. **API connectivity** - Check that backend is running on port 5041
4. **Build failures** - Run `npm run type-check` to identify TypeScript issues

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License.

##  Acknowledgments

- Mathematical rendering: [KaTeX](https://katex.org/)
- Mathematical input: [MathLive](https://cortexjs.io/mathlive/)
- Rich text editing: [Lexical](https://lexical.dev/)
- Graphing: [Geogebra](https://www.geogebra.org)
- Sketching: [Excalidraw](https://excalidraw.com/)
- UI Components: [Material-UI](https://mui.com/)

---

**Enhanced with collaborative features, user authentication, and advanced document management** 
