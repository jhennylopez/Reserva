USE SistemaReservasBD;
GO

-- ==============================================================================
-- 1. Insertar ADMINISTRADORES (No dependen de ninguna tabla)
-- ==============================================================================
INSERT INTO Administrador (Ci, nombres, apellidos, correo, telefono, contrasena)
VALUES 
('0912345678', 'Carlos', 'Mendoza', 'carlos.admin@airbnb.ec', '0991234567', '$2a$11$DummyHashCodeForPassword1'),
('1723456789', 'Andrea', 'Velasco', 'andrea.v@airbnb.ec', '0987654321', '$2a$11$DummyHashCodeForPassword2');
GO

-- ==============================================================================
-- 2. Insertar HUÉSPEDES (No dependen de ninguna tabla)
-- ==============================================================================
INSERT INTO Huesped (Ci, nombres, apellidos, correo, telefono, contrasena)
VALUES 
('0102345678', 'Juan', 'Pérez', 'juan.perez@gmail.com', '0971112233', '$2a$11$DummyHashCodeForPassword3'),
('1103456789', 'María', 'Gómez', 'mariagomez@outlook.com', '0962223344', '$2a$11$DummyHashCodeForPassword4'),
('0923456781', 'Luis', 'Torres', 'luis.t_89@yahoo.com', '0953334455', '$2a$11$DummyHashCodeForPassword5');
GO

-- ==============================================================================
-- 3. Insertar ALOJAMIENTOS (Dependen de Administrador)
-- ==============================================================================
INSERT INTO Alojamiento (descripcion, ubicacion, max_huespedes, num_habitaciones, num_banos, Id_administrador)
VALUES 
('Cabaña rústica con vista al volcán', 'Baños de Agua Santa, Tungurahua', 6, 3, 2, 1), -- Admin 1 (Carlos)
('Departamento moderno en el centro', 'La Carolina, Quito', 4, 2, 1, 2),            -- Admin 2 (Andrea)
('Casa de playa con piscina privada', 'Salinas, Santa Elena', 10, 5, 4, 1);           -- Admin 1 (Carlos)
GO

-- ==============================================================================
-- 4. Insertar IMÁGENES DE ALOJAMIENTOS (Dependen de Alojamiento)
-- ==============================================================================
INSERT INTO Imagen_Alojamiento (ruta_imagen, Id_alojamiento)
VALUES 
('/images/alojamientos/cabana_fachada.jpg', 1),
('/images/alojamientos/cabana_sala.jpg', 1),
('/images/alojamientos/cabana_cuarto1.jpg', 1),

('/images/alojamientos/depa_quito_vista.png', 2),
('/images/alojamientos/depa_quito_cocina.png', 2),

('/images/alojamientos/casa_playa_piscina.jpg', 3),
('/images/alojamientos/casa_playa_exterior.jpg', 3);
GO

-- ==============================================================================
-- 5. Insertar RESERVAS (Dependen de Huésped y Alojamiento)
-- Asegurando que la fecha_salida sea mayor a fecha_ingreso
-- ==============================================================================
INSERT INTO Reserva (fecha_ingreso, fecha_salida, numero_personas, tipo, Id_huesped, Id_alojamiento)
VALUES 
('2026-06-15', '2026-06-20', 4, 'Completa', 1, 1),   -- Juan alquila la Cabaña (Admin 1)
('2026-07-01', '2026-07-05', 2, 'Privada', 2, 2),    -- María alquila el Depa (Admin 2)
('2026-08-10', '2026-08-15', 8, 'Completa', 3, 3),   -- Luis alquila la Casa en la Playa (Admin 1)
('2026-09-01', '2026-09-03', 2, 'Compartida', 1, 2); -- Juan alquila el Depa (Admin 2)
GO

