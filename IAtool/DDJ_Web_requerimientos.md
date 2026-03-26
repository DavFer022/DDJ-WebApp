<img src="https://r2cdn.perplexity.ai/pplx-full-logo-primary-dark%402x.png" style="height:64px;margin-right:32px"/>

# Actúa como un arquitecto de software senior especializado en desarrollo de aplicaciones web de comercio electrónico.

Necesito que generes un plan de trabajo detallado para desarrollar una aplicación web para una tienda de ropa. La aplicación debe incluir dos áreas principales:

1. Panel de administración (para el equipo interno):

- Gestión de inventario (productos, tallas, colores, stock)
- Gestión de ventas
- Gestión de pedidos
- Gestión de clientes
- Gestión de pedidos personalizados de sublimación/DTF (con opciones como subida de diseños, especificaciones del cliente, estados del pedido, etc.)

2. Panel de cliente (frontend público):

- Visualización de catálogo de productos
- Filtros (talla, precio, categoría, etc.)
- Carrito de compras
- Proceso de checkout
- Registro e inicio de sesión
- Seguimiento de pedidos
- Solicitud de productos personalizados (DTF/sublimación)

Quiero que el resultado sea únicamente la estructura del proyecto, basada en aplicaciones de comercio electrónico modernas (como referencia conceptual tipo Shopify, Amazon o tiendas online similares), sin explicaciones extensas.

Incluye:

- Arquitectura general (frontend, backend, base de datos)
- Estructura de carpetas (frontend y backend)
- Módulos principales del sistema
- Entidades/modelos de base de datos
- Lista de data models mínimos necesarios para el funcionamiento
- APIs necesarias (endpoints principales)
- Roles de usuario y permisos
- Flujo general de funcionamiento
- Sitemap del sitio con jerarquía tipo árbol
- Funcionalidades clave organizadas por fases (MVP, V1, V2)

Restricciones:

- No incluir cronogramas
- No incluir backlog detallado ni historias de usuario
- No incluir UI Kit (temas, tipografía, wireframes)
- No incluir contenido ficticio

Formato de salida:

- Texto plano
- Bien organizado con encabezados y listas
- Sin ejemplos de código
- Sin introducción ni conclusión

Arquitectura general

- Frontend: SPA/SSR (React/Vue/Next/Nuxt) consumiendo APIs REST/JSON sobre HTTPS.[^1][^2]
- Backend: API modular (monolito modular o “headless” ligero) con servicios de autenticación, catálogo, carrito, pedidos, pagos y gestión de personalizaciones DTF/sublimación.[^3][^4][^2]
- Base de datos: Relacional (PostgreSQL/MySQL) para entidades transaccionales (productos, pedidos, clientes, stock) + opcional almacén de archivos (S3/Blob) para diseños subidos.[^5][^6][^3]

Estructura de carpetas – Frontend

- /frontend
    - /public
    - /src
        - /assets
        - /components
            - /common
            - /layout
            - /ui
            - /forms
            - /admin
            - /catalog
            - /cart
            - /checkout
            - /account
            - /orders
            - /custom-orders
        - /pages (o /routes)
            - /home
            - /catalog
            - /product
            - /cart
            - /checkout
            - /account
            - /orders
            - /custom
            - /auth
            - /admin
        - /modules
            - /catalog
            - /cart
            - /checkout
            - /orders
            - /customers
            - /custom-orders
            - /auth
        - /hooks
        - /context (o /store para estado global)
        - /services (clientes API)
        - /utils
        - /config

Estructura de carpetas – Backend

- /backend
    - /src
        - /config
        - /modules
            - /auth
            - /users
            - /roles-permissions
            - /products
            - /categories
            - /variants (tallas/colores)
            - /inventory
            - /pricing-promotions
            - /customers
            - /carts
            - /orders
            - /payments
            - /shipping
            - /custom-orders (DTF/sublimación)
            - /files (gestión de diseños)
            - /analytics-reports
            - /notifications (email/whatsapp/sms)
        - /routes
        - /controllers
        - /services
        - /repositories
        - /entities (o /models)
        - /middlewares
        - /utils
    - /tests

Módulos principales del sistema

- Autenticación y autorización
- Gestión de usuarios, roles y permisos
- Catálogo de productos (productos, categorías, colecciones, variantes)
- Inventario y stock
- Carrito de compras
- Checkout, pagos y envíos
- Gestión de pedidos estándar
- Gestión de clientes
- Gestión de pedidos personalizados DTF/sublimación
- Gestión de archivos/diseños subidos
- Reportes de ventas, pedidos, inventario
- Notificaciones (confirmación de pedido, estado de envío y de pedidos personalizados)

Entidades / modelos de base de datos

- User
- Role
- Permission
- UserRole (relación N:M)
- RolePermission (relación N:M)
- CustomerProfile (datos extra del cliente si se separa de User)
- Product
- ProductCategory
- ProductCategoryHierarchy (si se necesitan categorías en árbol)
- ProductVariant (talla, color, SKU específico)[^6]
- ProductImage
- Price (histórico / pricing por lista opcional)[^6]
- InventoryItem (stock por variante y almacén si aplica)[^5][^6]
- Cart
- CartItem
- Order
- OrderItem
- Payment
- PaymentTransaction
- ShippingAddress
- BillingAddress
- Shipment (envío asociado al pedido)
- ShipmentTracking
- CustomOrder (pedido personalizado DTF/sublimación)
- CustomOrderItem
- CustomDesignFile (archivo subido)
- CustomSpecification (texto, notas, instrucciones)
- CustomOrderStatusHistory
- CustomerMeasurement/Preferences (si se necesitan preferencias de talla/ajuste)
- AuditLog (eventos clave: cambios de estado, inventario, etc.)
- NotificationLog (envíos de email/sms)

Lista de data models mínimos necesarios

- User, Role, Permission, UserRole, RolePermission
- CustomerProfile
- Product, ProductCategory, ProductVariant, ProductImage
- InventoryItem
- Cart, CartItem
- Order, OrderItem
- Payment
- ShippingAddress, BillingAddress
- Shipment, ShipmentTracking
- CustomOrder, CustomOrderItem, CustomDesignFile, CustomSpecification, CustomOrderStatusHistory
- NotificationLog

APIs necesarias – endpoints principales (prefijo /api)

Autenticación y usuarios

- POST /auth/register
- POST /auth/login
- POST /auth/logout
- POST /auth/refresh-token
- GET /auth/me
- GET /users (solo admin)
- GET /users/:id (solo admin)
- PATCH /users/:id (solo admin)
- GET /roles (solo admin)
- POST /roles (solo admin)

Catálogo

- GET /products
- GET /products/:id
- GET /products/slug/:slug
- GET /categories
- GET /categories/:id
- GET /products?category=:id\&size=:size\&color=:color\&price_min=\&price_max= (filtros)
- GET /products/:id/variants
- GET /products/:id/images

Inventario

- GET /inventory (solo admin)
- GET /inventory/product/:productId (solo admin)
- PATCH /inventory/variant/:variantId (solo admin)

Carrito

- GET /cart (por usuario o por token de sesión)[^7][^6]
- POST /cart/items
- PATCH /cart/items/:id
- DELETE /cart/items/:id
- DELETE /cart (vaciar)

Checkout, pedidos y pagos

- POST /checkout/summary (calcula total, impuestos, envío)[^6]
- POST /checkout/place-order
- GET /orders (cliente autenticado ve los suyos, admin ve todos)
- GET /orders/:id
- PATCH /orders/:id/status (solo admin)
- POST /payments/create-session (integración pasarela)
- POST /payments/webhook (notificaciones de pago)

Clientes

- GET /customers (solo admin)
- GET /customers/:id (solo admin)
- GET /me/profile (cliente)
- PATCH /me/profile (cliente)
- GET /me/addresses
- POST /me/addresses
- PATCH /me/addresses/:id
- DELETE /me/addresses/:id

Envíos

- GET /shipping/methods (checkout)
- GET /shipments (solo admin)
- GET /shipments/:id
- PATCH /shipments/:id/status (solo admin)

Pedidos personalizados DTF/sublimación

- POST /custom-orders (solicitud de pedido personalizado)[^8]
- GET /custom-orders (cliente ve sus pedidos personalizados, admin ve todos)
- GET /custom-orders/:id
- PATCH /custom-orders/:id/status (solo admin)
- POST /custom-orders/:id/files (subir diseño)
- DELETE /custom-orders/:id/files/:fileId
- PATCH /custom-orders/:id/specifications (actualizar especificaciones)
- POST /custom-orders/:id/quote (generar cotización/monto estimado)
- POST /custom-orders/:id/confirm (cliente acepta cotización y pasa a pedido/pago)

Reportes y analítica (admin)

- GET /reports/sales/daily
- GET /reports/sales/by-product
- GET /reports/orders/status
- GET /reports/inventory/low-stock

Notificaciones

- POST /notifications/test (solo admin)
- GET /notifications (logs, solo admin)

Roles de usuario y permisos

- SuperAdmin: acceso total, configuración global, roles, usuarios, reportes avanzados.
- Admin: gestión de catálogo, inventario, pedidos, clientes, pedidos personalizados, reportes operativos.
- Staff/Operator: gestión operativa limitada (preparar pedidos, actualizar estados, gestionar inventario).
- Designer/DTF: acceso a módulo de pedidos personalizados, diseños, estado de producción.
- Customer (cliente autenticado): compras, gestión de cuenta, direcciones, pedidos estándar y personalizados.
- Guest (visitante): navegación catálogo, filtros, carrito anónimo, inicio de checkout antes de registro.[^4][^2]

Flujo general de funcionamiento

Flujo cliente – compra estándar

1. Visitante navega el catálogo, filtra por categoría, talla, color, precio.[^4]
2. Añade productos/variantes al carrito (asociado a sesión anónima o usuario).[^7][^6]
3. Se registra o inicia sesión (opcionalmente al principio o justo antes de pagar).
4. Revisa carrito, selecciona dirección de envío y método de entrega.
5. El sistema calcula totales, impuestos y costos de envío.[^6]
6. Cliente confirma y se redirige a la pasarela de pago.
7. Pasarela confirma pago (webhook), backend marca pedido como pagado y crea Shipment inicial.[^3][^5]
8. Cliente puede seguir el estado del pedido desde su panel.

Flujo cliente – pedido personalizado DTF/sublimación

1. Cliente accede a sección de productos personalizados.[^8]
2. Completa formulario con tipo de prenda, cantidad, tallas, colores, área de impresión, notas.
3. Sube archivos de diseño (imágenes vectoriales o raster).[^8]
4. Sistema registra CustomOrder en estado “Pendiente revisión/cotización”.
5. Staff/Designer revisa especificaciones y diseño, define precio y plazos, envía cotización.
6. Cliente revisa cotización, acepta y procede a pago (se genera pedido asociado).
7. Producción actualiza estados (En diseño, En impresión, En acabado, Enviado).
8. Cliente sigue el estado desde su panel y recibe notificaciones en cada cambio.

Flujo admin – gestión operativa

1. Admin/Staff gestiona catálogo: crea/edita productos, variantes y categorías.[^4]
2. Admin controla inventario: ajusta stock, revisa bajo stock, bloquea productos si no hay disponibilidad.[^5]
3. Admin gestiona pedidos estándar: prepara, marca enviados, actualiza tracking.
4. Designer/Staff gestiona pedidos personalizados: revisa diseños, cotiza, actualiza estados de producción.[^8]
5. Admin revisa reportes de ventas, pedidos y stock para toma de decisiones.[^3][^6]

Sitemap – jerarquía tipo árbol

- /
    - /catalogo
        - /catalogo/categoria/:categorySlug
        - /catalogo/producto/:productSlug
    - /ofertas (opcional)
    - /carrito
    - /checkout
    - /cuenta
        - /cuenta/perfil
        - /cuenta/direcciones
        - /cuenta/pedidos
            - /cuenta/pedidos/:orderId
        - /cuenta/pedidos-personalizados
            - /cuenta/pedidos-personalizados/:customOrderId
    - /pedidos/seguimiento/:trackingCode
    - /personalizado
        - /personalizado/crear-solicitud
        - /personalizado/:customOrderId
    - /auth
        - /auth/login
        - /auth/registro
        - /auth/recuperar-password
    - /informacion
        - /informacion/envios
        - /informacion/devoluciones
        - /informacion/contacto
- /admin
    - /admin/dashboard
    - /admin/productos
        - /admin/productos/nuevo
        - /admin/productos/:productId/editar
    - /admin/categorias
    - /admin/inventario
    - /admin/pedidos
        - /admin/pedidos/:orderId
    - /admin/clientes
        - /admin/clientes/:customerId
    - /admin/pedidos-personalizados
        - /admin/pedidos-personalizados/:customOrderId
    - /admin/reportes
        - /admin/reportes/ventas
        - /admin/reportes/inventario
    - /admin/configuracion
        - /admin/configuracion/usuarios
        - /admin/configuracion/roles

Funcionalidades clave por fases

MVP

- Frontend público:
    - Catálogo básico (lista de productos, detalle de producto, imágenes, tallas/colores).[^4]
    - Filtros esenciales (categoría, precio, talla, color).
    - Carrito de compras funcional (visitante y usuario autenticado).[^7][^6]
    - Checkout básico con cálculo de total y datos de envío.[^6]
    - Registro, login, perfil mínimo de cliente.
    - Visualización y seguimiento básico de pedidos del cliente.
- Backend / Admin:
    - CRUD de productos, categorías y variantes.[^4]
    - Gestión básica de inventario por variante.[^5]
    - Gestión de pedidos estándar (cambiar estado, marcar enviado, tracking).
    - Modelo inicial de pedidos personalizados DTF/sublimación (crear solicitud con archivos y texto, estados básicos).[^8]
    - Integración mínima con pasarela de pago (pago único) y webhooks.[^3]

V1

- Frontend público:
    - Mejora de filtros y búsqueda (por nombre, código, categoría jerárquica).[^4]
    - Historial de pedidos detallado, reordenar desde pedido previo.
    - Panel específico de pedidos personalizados con timeline de estados.[^8]
- Backend / Admin:
    - Roles y permisos granulares (admin, staff, designer).
    - Reportes operativos de ventas y pedidos (por rango de fechas, por producto).[^6]
    - Gestión avanzada de pedidos personalizados (cotización, aprobación, integración con flujo de producción).[^8]
    - Notificaciones por email para confirmaciones de pedidos y cambios de estado.[^3]

V2

- Frontend público:
    - Recomendaciones básicas de productos (relacionados, más vendidos por categoría).[^9]
    - Listas de deseos (wishlist).
    - Guardar diseños y configuraciones de personalizados como plantillas.[^8]
- Backend / Admin:
    - Reportes avanzados (márgenes por producto, rotación de inventario, análisis de recurrencia de clientes).[^3][^6]
    - Integraciones adicionales (transportistas, facturación electrónica según país).
    - Reglas de pricing y promociones (descuentos por categoría, volumen, cupones).[^4]
    - Automatizaciones simples (alertas de bajo stock, recordatorios de carrito abandonado).[^5][^6]
<span style="display:none">[^10]</span>

<div align="center">⁂</div>

[^1]: https://dev.to/alisamir/modern-frontend-architecture-a-definitive-guide-for-scalable-web-applications-2mj3

[^2]: https://virtocommerce.com/blog/ecommerce-architecture

[^3]: https://www.aalpha.net/blog/ecommerce-backend-development/

[^4]: https://www.shopify.com/es/blog/arquitectura-web-de-ecommerce

[^5]: https://dev.to/fabric_commerce/how-do-you-design-a-shopping-cart-database-for-e-commerce-4oeh

[^6]: https://fabric.inc/blog/commerce/ecommerce-data-model

[^7]: https://www.reddit.com/r/rails/comments/u9emcm/data_models_for_online_store_seperate_cart_and/

[^8]: https://www.productsdesigner.com/blog/dtf-printing-the-game-changer-of-the-ecommerce-custom-business/

[^9]: https://openaccess.uoc.edu/bitstream/10609/88405/5/emilio89TFM0119memoria.pdf

[^10]: https://www.youtube.com/watch?v=tnwqGe2Mi4A

