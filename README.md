# HTA_DATT 🧩 --- Hệ thống Quản lý Công Việc (Task Management API)

## 📌 Giới thiệu

HTA_DATT là hệ thống API được xây dựng bằng **ASP.NET Core Web API**, hỗ
trợ quản lý: - Người dùng (User) - Công việc (TaskItem) - Đăng nhập --
phân quyền - Quản lý dữ liệu thông qua Entity Framework Core

Hệ thống được thiết kế theo hướng **modular**, dễ mở rộng, phù hợp để
tích hợp với: - Web MVC - Mobile App (Flutter, React Native) - SPA
(React, Angular, Vue) - Microservices trong tương lai

------------------------------------------------------------------------

# 🏗️ Kiến trúc hệ thống (Architecture)

Dự án áp dụng **Layered Architecture** kết hợp **Repository Pattern**:

    Presentation Layer (Controllers)
            ↓
    Application Layer (DTOs, Services, Validation)
            ↓
    Domain Layer (Entities, Models)
            ↓
    Infrastructure Layer (Repositories, EF Core)

### 📍 Các thành phần chính

#### ✔ Controllers

-   Nhận và xử lý HTTP Request
-   Trả dữ liệu theo chuẩn JSON
-   Gọi xuống Service → Repository

#### ✔ Services

-   Chứa logic nghiệp vụ
-   Validate dữ liệu
-   Chuyển đổi DTO ↔ Model

#### ✔ Repository

-   Tương tác với database thông qua EF Core
-   CRUD User và TaskItem
-   Tách biệt logic DB khỏi controller

#### ✔ Database Layer

-   Quản lý qua `AppDbContext`
-   Hỗ trợ migrations Code First

#### ✔ Middleware

-   Xử lý lỗi tập trung
-   JWT Authorization (nếu kích hoạt)
-   Logging

------------------------------------------------------------------------

# 🧩 ERD -- Sơ đồ quan hệ dữ liệu

## 👤 Bảng User

  Cột            Kiểu dữ liệu    Mô tả
  -------------- --------------- ----------------------
  Id             int (PK)        Khóa chính
  Username       nvarchar(50)    Tên đăng nhập
  PasswordHash   nvarchar(max)   Mật khẩu (đã mã hóa)
  Email          nvarchar(100)   Email
  CreatedAt      datetime        Ngày tạo

## 📝 Bảng TaskItem

  Cột           Kiểu dữ liệu         Mô tả
  ------------- -------------------- -------------------------------
  Id            int (PK)             Khóa chính
  Title         nvarchar(200)        Tên task
  Description   nvarchar(max)        Mô tả task
  Status        int                  0 = Todo, 1 = Doing, 2 = Done
  DueDate       datetime             Deadline
  UserId        int (FK → User.Id)   Người thực hiện

## 🔗 Quan hệ

    User (1) ----- (∞) TaskItem

------------------------------------------------------------------------

# 🚀 API Documentation

## 🔐 Auth API

### **Đăng nhập**

**POST** `/api/auth/login`

**Request**

``` json
{
  "username": "admin",
  "password": "123456"
}
```

**Response**

``` json
{
  "success": true,
  "token": "jwt-token",
  "user": {
    "id": 1,
    "username": "admin"
  }
}
```

------------------------------------------------------------------------

# 👤 User API

  Method   Endpoint           Mô tả
  -------- ------------------ ----------------------
  GET      `/api/user`        Lấy danh sách user
  GET      `/api/user/{id}`   Lấy thông tin 1 user
  POST     `/api/user`        Thêm user mới
  PUT      `/api/user/{id}`   Cập nhật user
  DELETE   `/api/user/{id}`   Xóa user

------------------------------------------------------------------------

# 📝 Task API

  Method   Endpoint           Mô tả
  -------- ------------------ --------------------
  GET      `/api/task`        Lấy danh sách task
  GET      `/api/task/{id}`   Lấy task theo ID
  POST     `/api/task`        Tạo task mới
  PUT      `/api/task/{id}`   Cập nhật task
  DELETE   `/api/task/{id}`   Xóa task

------------------------------------------------------------------------

# 🐳 Docker Support (nếu cần)

Chạy API bằng Docker:

``` bash
docker build -t datt-api .
docker run -p 5000:80 datt-api
```

Hoặc dùng docker-compose:

``` bash
docker-compose up --build
```

------------------------------------------------------------------------

# ⚙️ Chạy dự án

## 1️⃣ Clone repo

``` bash
git clone https://github.com/AEN25/HTA_DATT.git
cd HTA_DATT
```

## 2️⃣ Cấu hình DB

Sửa `appsettings.json` → ConnectionStrings

## 3️⃣ Tạo database

``` bash
dotnet ef database update
```

## 4️⃣ Run API

``` bash
dotnet run --project DATT.API
```

------------------------------------------------------------------------

# 👥 Đóng góp

Pull Request luôn được hoan nghênh.\
Vui lòng: - Fork repo\
- Tạo nhánh mới\
- Commit & mở PR

------------------------------------------------------------------------

# 📄 License

Bạn có thể thêm MIT License nếu muốn mở mã nguồn.

------------------------------------------------------------------------

# ✨ Thành viên phát triển

-   **Hoàng Tiến Anh**

------------------------------------------------------------------------

# 🎯 Kết luận

HTA_DATT là API nền tảng mạnh mẽ để xây dựng hệ thống quản lý công việc,
với: - Kiến trúc rõ ràng\
- API chuẩn REST\
- Dữ liệu quan hệ đầy đủ\
- Dễ tích hợp & mở rộng
