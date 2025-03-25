# Blog API

## Overview
We are building a simple Blog using .Net and EFCore, as well as JWT Token for Authentification

## Features

**User Creation and Authentification (Login)** using JWT Tokens
**Create, Update, Publish, and Unpublish our Blogs**
**Soft Delete Blogs** (This will keep them hidden but still in our system just in case the FBI needs :3)
**Search Blogs** (By Categories)

## EndPoints
# User

-   `POST /User/CreateUser`
-   `POST /User/Login`
-   `GET /User/GetUserInfo`

# Blog

-   `POST /Blog/AddBlog`
-   `GET /Blog/GetAllBlogs`
-   `PUT /Blog/EditBlog`
-   `GET /Blog/GetBlogByUserID/{userID}`
-   `DELETE /Blog/DeleteBlog`
-   `GET /BLog/GetBlogByCategory/{category}`


## Async Methods in C#

Async Methods in C# are used when queries databases without blockingour main thread

This leads to better performances *Allows us to handle more than one request*