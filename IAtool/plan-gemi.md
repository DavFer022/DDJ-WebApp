# Plan de Ejecución - DDJ-Web

Este plan detalla los pasos para la implementación de la tienda de ropa con personalización DTF/Sublimación, utilizando .NET 10 para el Backend y Next.js para el Frontend.

## 1. Fase MVP (Producto Mínimo Viable)
**Objetivo:** Tener una tienda funcional que permita navegar, comprar productos estándar y solicitar personalizados básicos.

### Backend (.NET 10)
- **Capa de Dominio:** Definir entidades base (`Product`, `Category`, `ProductVariant`, `User`, `Order`, `CustomOrder`).
- **Capa de Infraestructura:** 
    - Configurar Entity Framework Core con PostgreSQL/MySQL.
    - Implementar Repositorios básicos.
    - Configurar Identity para autenticación JWT.
- **Capa de Aplicación:** Casos de uso para listado de catálogo y gestión de pedidos.
- **API:**
    - Endpoints de autenticación (Login/Registro).
    - Endpoints de Catálogo (lectura para clientes, CRUD para admin).
    - Endpoints de Pedidos (creación y seguimiento básico).

### Frontend (Next.js)
- **Inicialización:** Crear proyecto Next.js en la carpeta `/Frontend`.
- **Estructura:** Configurar carpetas `/src/components`, `/src/app`, `/src/services`.
- **Módulos:**
    - Catálogo: Home y detalle de producto.
    - Carrito: Estado global y persistencia local.
    - Auth: Formularios de acceso y registro.
    - Checkout: Flujo de envío y confirmación de orden.

---

## 2. Fase V1 (Operatividad Avanzada)
**Objetivo:** Mejorar la gestión administrativa y el flujo de pedidos personalizados.

### Backend
- **Gestión de Archivos:** Integrar servicio para subida de diseños (Local o Azure Blob/AWS S3).
- **Módulo Custom:** Lógica de cotización y estados de producción para DTF.
- **Roles:** Implementar políticas de autorización (Admin, Designer, Customer).
- **Notificaciones:** Integrar servicio de envío de correos (SendGrid/SMTP).

### Frontend
- **Panel Admin:** 
    - Dashboard de métricas básicas.
    - Gestión de inventario y stock por variantes.
    - Listado y actualización de pedidos.
- **Panel de Pedidos Personalizados:** Interfaz para subir diseños y ver estados de producción.
- **Búsqueda y Filtros:** Implementar filtros avanzados en el catálogo.

---

## 3. Fase V2 (Optimización y Automatización)
**Objetivo:** Mejorar la experiencia de usuario y automatizar procesos administrativos.

### Backend
- **Reportes:** Generación de reportes de ventas e inventario bajo (Excel/PDF).
- **Promociones:** Motor de reglas para cupones y descuentos.
- **Integraciones:** Conexión con transportistas para tracking en tiempo real.

### Frontend
- **Wishlist:** Lista de deseos por usuario.
- **Recomendaciones:** Algoritmo básico de productos relacionados.
- **Plantillas:** Guardar configuraciones de personalizados previos.

---

## Verificación y Pruebas
- **Unit Testing:** Pruebas de lógica de negocio en `DDJ.Application`.
- **Integración:** Validar flujo completo desde carrito Next.js hasta persistencia en DB.
- **Seguridad:** Auditoría de endpoints protegidos y manejo de archivos.
- **Performance:** Optimización de imágenes en Next.js y queries eficientes en EF Core.
