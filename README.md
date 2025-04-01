## 📋 Overview

FileUploader.Api is a backend service that enables users to upload, organize, and securely share files.

## ✨ Features

- **Secure Authentication**: JWT-based authentication system with refresh tokens
- **File Management**: Upload, download, and delete files
- **Folder Organization**: Create and manage folders to organize files
- **Secure File Sharing**: Generate time-limited share tokens for files
- **Cloud Storage**: Integration with Cloudinary for reliable file storage
- **Cross-Origin Support**: Configured CORS policies for secure cross-domain operations

## 🛠️ Technologies

- **ASP.NET Core 9.0**
- **Entity Framework Core**
- **PostgreSQL**
- **Cloudinary**
- **JWT Authentication**

## 📝 API Endpoints

### Authentication

- `POST /api/auth/register`: Register a new user
- `POST /api/auth/login`: Login and get JWT token
- `POST /api/auth/refresh-token`: Refresh JWT token
- `GET /api/auth/check-username`: Check if username is available

### Files

- `POST /api/file/upload`: Upload a file to a folder
- `GET /api/file/download/{id}`: Download a file by ID (authenticated)
- `GET /api/file/download/shared/{token}`: Download a shared file using token (anonymous)
- `POST /api/file/share/{id}`: Generate share token for a file
- `DELETE /api/file/delete/{id}`: Delete a file
- `GET /api/file/folder/{folderId}`: Get all files in a folder

### Folders

- `POST /api/folder/create`: Create a new folder
- `DELETE /api/folder/{id}`: Delete a folder
- `GET /api/folder/subfolders/{id}`: Get subfolders of a specified folder
- `GET /api/folder/root`: Get all folders at root level
