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
- **PostgreSQL**information
- **Cloudinary**
- **JWT Authentication**

```


## 📝 API Endpoints

### Authentication

- `POST /api/auth/register`: Register a new user
- `POST /api/auth/login`: Login and get JWT token
- `POST /api/auth/refresh`: Refresh JWT token

### Files

- `POST /api/file/upload`: Upload a file to a folder
- `GET /api/file/download/{id}`: Download a file by ID (authenticated)
- `GET /api/file/shared/{token}`: Download a shared file using token (anonymous)
- `POST /api/file/share/{id}`: Generate share token for a file
- `DELETE /api/file/delete/{id}`: Delete a file

### Folders

- `POST /api/folder`: Create a new folder
- `GET /api/folder`: Get all folders for current user
- `GET /api/folder/{id}/files`: Get all files in a folder
- `DELETE /api/folder/{id}`: Delete a folder

```
