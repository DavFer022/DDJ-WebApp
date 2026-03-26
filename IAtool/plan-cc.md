# Plan de Ejecución – DDJ Web (Tienda de Ropa)

## Stack Tecnológico

| Capa                       | Tecnología                           |
| -------------------------- | ------------------------------------ |
| Frontend                   | Next.js 14+ (App Router, TypeScript) |
| Backend                    | .NET 10 (C#, Clean Architecture)     |
| Base de datos              | PostgreSQL                           |
| ORM                        | Entity Framework Core                |
| Autenticación              | JWT + Refresh Tokens                 |
| Almacenamiento de archivos | Cloudinary                           |
| Despliegue Backend         | Railway + Docker                     |
| Despliegue Frontend        | Vercel                               |

---

## Arquitectura General

```
Cliente (Browser)
    │
    ▼
Next.js (Frontend SSR/SSG)
    │  REST/JSON sobre HTTPS (/api)
    ▼
.NET 10 Web API  ──►  PostgreSQL
    │
    └──► Cloudinary (Imágenes/Diseños DTF-Sublimación)
```

**Backend – Clean Architecture (.NET 10)**

```
Backend/
├── DDJ.sln
├── DDJ.Domain/           # Entidades, interfaces, value objects
├── DDJ.Application/      # Casos de uso, DTOs, servicios de aplicación
├── DDJ.Infrastructure/   # EF Core, repositorios, servicios externos (Cloudinary, etc.)
└── DDJ.Api/              # Controllers, middlewares, configuración
```

**Frontend – Next.js (App Router)**

```
frontend/
├── public/
└── src/
    ├── app/                    # Rutas (App Router)
    │   ├── (store)/            # Layout tienda pública
    │   │   ├── page.tsx        # Home /
    │   │   ├── catalogo/
    │   │   ├── carrito/
    │   │   ├── checkout/
    │   │   ├── cuenta/
    │   │   ├── pedidos/
    │   │   └── personalizado/
    │   ├── auth/
    │   └── admin/              # Layout panel admin
    │       ├── dashboard/
    │       ├── productos/
    │       ├── categorias/
    │       ├── inventario/
    │       ├── pedidos/
    │       ├── clientes/
    │       ├── pedidos-personalizados/
    │       ├── reportes/
    │       └── configuracion/
    ├── components/
    │   ├── ui/                 # Componentes base (Button, Input, Modal…)
    │   ├── layout/             # Header, Footer, Sidebar
    │   ├── catalog/
    │   ├── cart/
    │   ├── checkout/
    │   ├── orders/
    │   ├── custom-orders/
    │   └── admin/
    ├── lib/
    │   ├── api/                # Clientes fetch hacia .NET API
    │   └── utils/
    ├── hooks/
    ├── context/                # CartContext, AuthContext
    └── types/                  # Tipos TypeScript compartidos
```

---

## Módulos Principales

1. Autenticación y autorización (JWT)
2. Gestión de usuarios, roles y permisos
3. Catálogo de productos, categorías y variantes
4. Inventario y stock
5. Carrito de compras (anónimo y autenticado)
6. Checkout, pagos y métodos de envío
7. Gestión de pedidos estándar
8. Gestión de clientes
9. Pedidos personalizados DTF/sublimación
10. Gestión de archivos/diseños subidos
11. Reportes de ventas, pedidos e inventario
12. Notificaciones (email)

---

## Entidades / Modelos de Base de Datos

### Identidad y acceso

- `User` – `Role` – `Permission` – `UserRole` – `RolePermission`

### Clientes

- `CustomerProfile`

### Catálogo

- `Product` – `ProductCategory` – `ProductCategoryHierarchy`
- `ProductVariant` (talla, color, SKU)
- `ProductImage`
- `Price`

### Inventario

- `InventoryItem` (stock por variante)

### Carrito y pedidos

- `Cart` – `CartItem`
- `Order` – `OrderItem`
- `Payment` – `PaymentTransaction`

### Direcciones y envíos

- `ShippingAddress` – `BillingAddress`
- `Shipment` – `ShipmentTracking`

### Pedidos personalizados

- `CustomOrder` – `CustomOrderItem`
- `CustomDesignFile`
- `CustomSpecification`
- `CustomOrderStatusHistory`

### Auditoría y notificaciones

- `AuditLog`
- `NotificationLog`

---

## APIs – Endpoints Principales (`/api`)

### Auth & Usuarios

| Método    | Endpoint              | Acceso  |
| --------- | --------------------- | ------- |
| POST      | `/auth/register`      | Público |
| POST      | `/auth/login`         | Público |
| POST      | `/auth/logout`        | Auth    |
| POST      | `/auth/refresh-token` | Auth    |
| GET       | `/auth/me`            | Auth    |
| GET       | `/users`              | Admin   |
| GET/PATCH | `/users/{id}`         | Admin   |
| GET/POST  | `/roles`              | Admin   |

### Catálogo

| Método              | Endpoint                                                               |
| ------------------- | ---------------------------------------------------------------------- |
| GET                 | `/products` (con filtros: category, size, color, price_min, price_max) |
| GET                 | `/products/{id}`                                                       |
| GET                 | `/products/slug/{slug}`                                                |
| GET/POST/PUT/DELETE | `/products` (CRUD admin)                                               |
| GET                 | `/products/{id}/variants`                                              |
| GET                 | `/products/{id}/images`                                                |
| GET/POST/PUT/DELETE | `/categories`                                                          |

### Inventario (Admin)

| Método | Endpoint                         |
| ------ | -------------------------------- |
| GET    | `/inventory`                     |
| GET    | `/inventory/product/{productId}` |
| PATCH  | `/inventory/variant/{variantId}` |

### Carrito

| Método | Endpoint           |
| ------ | ------------------ |
| GET    | `/cart`            |
| POST   | `/cart/items`      |
| PATCH  | `/cart/items/{id}` |
| DELETE | `/cart/items/{id}` |
| DELETE | `/cart`            |

### Checkout, Pedidos y Pagos

| Método | Endpoint                      |
| ------ | ----------------------------- |
| POST   | `/checkout/summary`           |
| POST   | `/checkout/place-order`       |
| GET    | `/orders`                     |
| GET    | `/orders/{id}`                |
| PATCH  | `/orders/{id}/status` (Admin) |
| POST   | `/payments/create-session`    |
| POST   | `/payments/webhook`           |

### Clientes (perfil propio)

| Método                | Endpoint        |
| --------------------- | --------------- |
| GET/PATCH             | `/me/profile`   |
| GET/POST/PATCH/DELETE | `/me/addresses` |

### Clientes (Admin)

| Método | Endpoint          |
| ------ | ----------------- |
| GET    | `/customers`      |
| GET    | `/customers/{id}` |

### Envíos

| Método    | Endpoint             |
| --------- | -------------------- |
| GET       | `/shipping/methods`  |
| GET/PATCH | `/shipments` (Admin) |

### Pedidos Personalizados DTF/Sublimación

| Método | Endpoint                             |
| ------ | ------------------------------------ |
| POST   | `/custom-orders`                     |
| GET    | `/custom-orders`                     |
| GET    | `/custom-orders/{id}`                |
| PATCH  | `/custom-orders/{id}/status` (Admin) |
| POST   | `/custom-orders/{id}/files`          |
| DELETE | `/custom-orders/{id}/files/{fileId}` |
| PATCH  | `/custom-orders/{id}/specifications` |
| POST   | `/custom-orders/{id}/quote`          |
| POST   | `/custom-orders/{id}/confirm`        |

### Reportes (Admin)

| Método | Endpoint                       |
| ------ | ------------------------------ |
| GET    | `/reports/sales/daily`         |
| GET    | `/reports/sales/by-product`    |
| GET    | `/reports/orders/status`       |
| GET    | `/reports/inventory/low-stock` |

### Notificaciones (Admin)

| Método | Endpoint              |
| ------ | --------------------- |
| POST   | `/notifications/test` |
| GET    | `/notifications`      |

---

## Roles y Permisos

| Rol                | Acceso                                                                               |
| ------------------ | ------------------------------------------------------------------------------------ |
| **SuperAdmin**     | Acceso total: configuración, roles, usuarios, reportes avanzados                     |
| **Admin**          | Catálogo, inventario, pedidos, clientes, pedidos personalizados, reportes operativos |
| **Staff/Operator** | Gestión operativa: estados de pedidos, inventario                                    |
| **Designer/DTF**   | Módulo de pedidos personalizados, diseños, estados de producción                     |
| **Customer**       | Compras, cuenta, direcciones, pedidos estándar y personalizados                      |
| **Guest**          | Catálogo, filtros, carrito anónimo, inicio de checkout                               |

---

## Sitemap

```
/
├── /catalogo
│   ├── /catalogo/categoria/:categorySlug
│   └── /catalogo/producto/:productSlug
├── /ofertas
├── /carrito
├── /checkout
├── /cuenta
│   ├── /perfil
│   ├── /direcciones
│   ├── /pedidos
│   │   └── /:orderId
│   └── /pedidos-personalizados
│       └── /:customOrderId
├── /pedidos/seguimiento/:trackingCode
├── /personalizado
│   ├── /crear-solicitud
│   └── /:customOrderId
├── /auth
│   ├── /login
│   ├── /registro
│   └── /recuperar-password
└── /informacion
    ├── /envios
    ├── /devoluciones
    └── /contacto

/admin
├── /dashboard
├── /productos
│   ├── /nuevo
│   └── /:productId/editar
├── /categorias
├── /inventario
├── /pedidos
│   └── /:orderId
├── /clientes
│   └── /:customerId
├── /pedidos-personalizados
│   └── /:customOrderId
├── /reportes
│   ├── /ventas
│   └── /inventario
└── /configuracion
    ├── /usuarios
    └── /roles
```

---

## Fases de Desarrollo

### MVP

**Backend (.NET)**

- [ ] Setup solución Clean Architecture (Domain, Application, Infrastructure, Api)
- [ ] Configuración EF Core + PostgreSQL + migraciones iniciales
- [ ] Módulo Auth: registro, login, JWT, refresh token
- [ ] CRUD Productos, Categorías, Variantes
- [ ] Gestión básica de inventario por variante
- [ ] Módulo Carrito (anónimo + autenticado con merge al login)
- [ ] Checkout básico + cálculo de total + dirección de envío
- [ ] Integración pasarela de pago (webhook)
- [ ] Gestión de pedidos estándar (estados, tracking básico)
- [ ] Modelo inicial CustomOrder (crear solicitud + subir archivos + estados básicos)
- [ ] Integración con Cloudinary (subida de diseños)

**Frontend (Next.js)**

- [ ] Setup proyecto Next.js + TypeScript + estructura de carpetas
- [ ] Configuración de ESLint, Prettier, Tailwind CSS
- [ ] Layout base: Header, Footer, navegación
- [ ] Página Catálogo con filtros (categoría, precio, talla, color)
- [ ] Página Detalle de producto
- [ ] Carrito de compras (contexto global)
- [ ] Flujo Checkout (dirección, resumen, pago)
- [ ] Registro e inicio de sesión
- [ ] Panel de cuenta: perfil + pedidos básicos
- [ ] Panel Admin: CRUD productos, categorías, inventario, listado de pedidos

---

### V1

**Backend (.NET)**

- [ ] Roles y permisos granulares (middleware de autorización por rol)
- [ ] Gestión avanzada de pedidos personalizados (cotización, aprobación)
- [ ] Reportes operativos (ventas por fecha, por producto)
- [ ] Notificaciones por email (confirmaciones, cambios de estado)
- [ ] Endpoints de búsqueda avanzada y filtros combinados
- [ ] Paginación y ordenamiento en listados

**Frontend (Next.js)**

- [ ] Búsqueda por nombre, código, categoría jerárquica
- [ ] Historial de pedidos detallado + reordenar
- [ ] Panel de pedidos personalizados con timeline de estados
- [ ] Dashboard Admin con reportes básicos (gráficas)
- [ ] Gestión de clientes (Admin)
- [ ] Gestión avanzada de pedidos personalizados (Admin)

---

### V2

**Backend (.NET)**

- [ ] Reglas de pricing y promociones (cupones, descuentos por volumen)
- [ ] Reportes avanzados (márgenes, rotación de inventario, recurrencia)
- [ ] Integraciones adicionales (transportistas, facturación electrónica)
- [ ] Automatizaciones (alertas bajo stock, recordatorios carrito abandonado)

**Frontend (Next.js)**

- [ ] Recomendaciones de productos (relacionados, más vendidos)
- [ ] Wishlist / Lista de deseos
- [ ] Plantillas de diseños personalizados guardados
- [ ] Mejoras de UX/UI y performance (Core Web Vitals)

---

## Flujos Principales

### Compra Estándar

1. Visitante navega catálogo y filtra productos
2. Agrega variantes al carrito (sesión anónima)
3. Inicia sesión / se registra (carrito se fusiona)
4. Selecciona dirección y método de envío
5. Sistema calcula totales
6. Confirma y paga (redirige a pasarela)
7. Webhook confirma pago → pedido marcado como pagado → Shipment creado
8. Cliente sigue el estado desde su panel

### Pedido Personalizado DTF/Sublimación

1. Cliente accede a sección de personalizados
2. Completa formulario (prenda, cantidad, tallas, área de impresión, notas)
3. Sube archivos de diseño
4. Sistema crea `CustomOrder` en estado "Pendiente revisión"
5. Staff/Designer revisa, define precio y plazos, envía cotización
6. Cliente acepta cotización y procede al pago
7. Producción actualiza estados (En diseño → En impresión → En acabado → Enviado)
8. Cliente recibe notificaciones en cada cambio de estado

### Gestión Operativa (Admin)

1. Admin gestiona catálogo: crea/edita productos, variantes, categorías
2. Controla inventario: ajusta stock, revisa bajo stock
3. Gestiona pedidos estándar: prepara, marca enviados, actualiza tracking
4. Staff/Designer gestiona pedidos personalizados: revisa, cotiza, actualiza producción
5. Revisa reportes de ventas y stock

---

## Convenciones de Desarrollo

### Backend (.NET)

- Nombres en inglés para clases, métodos, propiedades
- Controllers delgados: lógica en Application layer (CQRS con MediatR recomendado)
- Repositorio genérico + repositorios específicos en Infrastructure
- DTOs para entrada/salida en Api layer (no exponer entidades de dominio)
- Validaciones con FluentValidation
- Respuestas estandarizadas: `{ data, message, success, errors }`

### Frontend (Next.js)

- Nombres de componentes en PascalCase
- Hooks en camelCase con prefijo `use`
- Rutas siguiendo el sitemap definido
- Fetching de datos con Server Components donde sea posible (SSR)
- Client Components solo donde se requiera interactividad
- Estado global con Context API (carrito, auth); Zustand para estados más complejos si se requiere
