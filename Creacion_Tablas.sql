-- 1. Crear la base de datos
CREATE DATABASE SistemaReservasBD;
GO

USE SistemaReservasBD;
GO

-- 2. Crear tabla ADMINISTRADOR
CREATE TABLE Administrador (
    Id_administrador INT IDENTITY(1,1) PRIMARY KEY,
    Ci VARCHAR(10) UNIQUE NOT NULL,
    nombres VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    correo VARCHAR(150) UNIQUE NOT NULL,
    telefono VARCHAR(20),
    contrasena VARCHAR(12) NOT NULL,
    fecha_registro DATE DEFAULT GETDATE(),

    CONSTRAINT CHK_Admin_CI CHECK (LEN(CI) = 10 AND CI NOT LIKE '%[^0-9]%')
);
GO

-- 3. Crear tabla HUESPED
CREATE TABLE Huesped (
    Id_huesped INT IDENTITY(1,1) PRIMARY KEY,
    Ci VARCHAR(10) UNIQUE NOT NULL,
    nombres VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    correo VARCHAR(150) UNIQUE NOT NULL,
    telefono VARCHAR(20),
    contrasena VARCHAR(12) NOT NULL,

    CONSTRAINT CHK_Huesped_CI CHECK (LEN(CI) = 10 AND CI NOT LIKE '%[^0-9]%')
);
GO

-- 4. Crear tabla ALOJAMIENTO (Con Cascada desde Administrador)
CREATE TABLE Alojamiento (
    Id_alojamiento INT IDENTITY(1,1) PRIMARY KEY,
    descripcion VARCHAR(500),
    ubicacion VARCHAR(255) NOT NULL,
    max_huespedes INT NOT NULL,
    num_habitaciones INT NOT NULL,
    num_banos INT NOT NULL,
    Id_administrador INT NOT NULL,
    precio_por_noche DECIMAL(10, 2) NOT NULL DEFAULT 0.00, -- Coma añadida aquí
    
    -- Al borrar el administrador, se borran sus alojamientos
    CONSTRAINT FK_Alojamiento_Admin FOREIGN KEY (Id_administrador) 
    REFERENCES Administrador(Id_administrador) ON DELETE CASCADE
);
GO

-- 5. Crear tabla RESERVA (Con Cascada desde Huesped y Alojamiento)
CREATE TABLE Reserva (
    Id_reserva INT IDENTITY(1,1) PRIMARY KEY,
    fecha_ingreso DATE NOT NULL,
    fecha_salida DATE NOT NULL,
    numero_personas INT NOT NULL,
    tipo VARCHAR(50) NOT NULL, 
    Id_huesped INT NOT NULL,
    Id_alojamiento INT NOT NULL,
    
    -- Al borrar el huésped, se borran sus reservas
    CONSTRAINT FK_Reserva_Huesped FOREIGN KEY (Id_huesped) 
    REFERENCES Huesped(Id_huesped) ON DELETE CASCADE,
    
    -- Al borrar el alojamiento, se borran las reservas asociadas
    CONSTRAINT FK_Reserva_Alojamiento FOREIGN KEY (Id_alojamiento) 
    REFERENCES Alojamiento(Id_alojamiento) ON DELETE CASCADE,

    CONSTRAINT CHK_Fechas_Reserva CHECK (fecha_salida > fecha_ingreso),
    CONSTRAINT CHK_Personas_Minimas CHECK (numero_personas > 0)
);
GO

-- 6. Crear tabla FACTURA (Con Cascada desde Reserva)
CREATE TABLE Factura (
    Id_factura INT IDENTITY(1,1) PRIMARY KEY,
    Id_reserva INT UNIQUE NOT NULL, -- Relación 1 a 1: Una reserva = Una factura
    fecha_emision DATETIME DEFAULT GETDATE(),
    dias_ocupacion INT NOT NULL,
    subtotal DECIMAL(10, 2) NOT NULL,
    impuestos DECIMAL(10, 2) NOT NULL, 
    total DECIMAL(10, 2) NOT NULL,
    
    -- Si la reserva se cancela/elimina, la factura también desaparece
    CONSTRAINT FK_Factura_Reserva FOREIGN KEY (Id_reserva) 
    REFERENCES Reserva(Id_reserva) ON DELETE CASCADE
);
GO

