# Identity API

A RESTful API for user authentication, stock management, portfolio tracking, and comment functionality.

## Table of Contents
- [API Overview](#api-overview)
- [Authentication](#authentication)
- [Endpoints](#endpoints)
  - [Account](#account)
  - [Comment](#comment)
  - [Portfolio](#portfolio)
  - [Stock](#stock)
- [Models](#models)
- [Security](#security)

## API Overview
- **Base URL**: `/api`
- **Version**: v1
- **Authentication**: JWT Bearer Token

## Authentication
All endpoints except `/api/login` and `/api/register` require authentication via JWT Bearer token.

### Login
```
POST /api/login
```
Logs in a user and returns a JWT token.

**Request Body**:
```json
{
  "userName": "string",
  "password": "string"
}
```

### Register
```
POST /api/register
```
Creates a new user account with "User" role.

**Request Body**:
```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

## Endpoints

### Account
| Endpoint | Method | Description | Auth Required |
|----------|--------|-------------|---------------|
| `/api/login` | POST | User login | No |
| `/api/register` | POST | User registration | No |

### Comment
| Endpoint | Method | Description | Auth Required |
|----------|--------|-------------|---------------|
| `/api/Comments` | GET | Get all comments | Yes |
| `/api/create/{stockId}` | POST | Add comment for a stock | Yes |
| `/api/Comments/{stockId}` | GET | Get comments for a stock | Yes |
| `/api/Comment/update/{commentId}` | PUT | Update a comment | Yes |
| `/api/Comments/users` | GET | Get user's comments | Yes |
| `/api/Comment/delete` | DELETE | Delete a comment | Yes |

### Portfolio
| Endpoint | Method | Description | Auth Required |
|----------|--------|-------------|---------------|
| `/api/Portfolio/portfolios` | GET | Get user's portfolio | Yes |
| `/api/Portfolio` | POST | Add stock to portfolio | Yes |
| `/api/Portfolio/delete/{stockSymbol}` | DELETE | Remove stock from portfolio | Yes |

### Stock
| Endpoint | Method | Description | Auth Required |
|----------|--------|-------------|---------------|
| `/api/Stock/create` | POST | Create new stock | Yes |
| `/api/Stock/{id}` | GET | Get stock by ID | Yes |
| `/api/Stock/{id}` | PUT | Update stock | Yes |
| `/api/Stock/{id}` | DELETE | Delete stock | Yes |
| `/api/Stock` | GET | Get all stocks | Yes |
| `/api/Stock/symbol/{symbol}` | GET | Get stock by symbol | Yes |

## Models

### Comment
```json
{
  "id": 0,
  "title": "string",
  "content": "string",
  "createdOn": "2023-01-01T00:00:00",
  "stockId": 0,
  "appUserId": "string"
}
```

### Stock
```json
{
  "id": 0,
  "symbol": "string",
  "companyName": "string",
  "purchase": 0,
  "lastDiv": 0,
  "industry": "string",
  "marketCap": 0
}
```

### Portfolio
```json
{
  "appUserId": "string",
  "stockId": 0
}
```

### User
```json
{
  "userName": "string",
  "email": "string",
  "token": "string"
}
```

## Security
All authenticated endpoints require a JWT Bearer token in the Authorization header:
```
Authorization: Bearer <your_token>
```

## Error Responses
Common error responses include:
- 400 Bad Request
- 401 Unauthorized
- 404 Not Found
- 500 Internal Server Error
```

This README provides a comprehensive overview of your API with clear sections for authentication, endpoints, models, and security. You may want to customize the base URL and add any additional deployment or setup instructions specific to your environment.