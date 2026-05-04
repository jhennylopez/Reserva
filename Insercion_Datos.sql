USE SistemaReservasBD;
GO

-- ==============================================================================
-- Insertar ADMINISTRADORES (No dependen de ninguna tabla)
-- ==============================================================================
INSERT INTO Administrador (Ci, nombres, apellidos, correo, telefono, contrasena)
VALUES 
('0605082007', 'Jhenny', 'López', 'jhenny.admin@airbnb.ec', '0991234567', 'Admin123') 

GO

-- ==============================================================================
-- Insertar HUÉSPEDES (No dependen de ninguna tabla)
-- ==============================================================================
INSERT INTO Huesped (Ci, nombres, apellidos, correo, telefono, contrasena)
VALUES 
('0102345678', 'Juan', 'Pérez', 'juan.perez@gmail.com', '0971112233', 'Juan123')
GO

-- ==============================================================================
-- 3. Insertar ALOJAMIENTOS (Dependen de Administrador)
-- ==============================================================================
INSERT INTO Alojamiento (descripcion, ubicacion, max_huespedes, num_habitaciones, num_banos, Id_administrador)
VALUES 
('Casa de tres pisos con vista increible de la ciudad. Completamente amoblada, a cinco minutos del centro de la ciudad. Con áreas de recreación, piscina,área de parrillada.', 'Baños de Agua Santa, Tungurahua', 15, 7, 4, 1)  
GO

-- ==============================================================================
-- Insertar RESERVAS (Dependen de Huésped y Alojamiento)
-- Asegurando que la fecha_salida sea mayor a fecha_ingreso
-- ==============================================================================
INSERT INTO Reserva (fecha_ingreso, fecha_salida, numero_personas, tipo, Id_huesped, Id_alojamiento)
VALUES 
('2026-07-01', '2026-07-05', 2, 'Privada', 1, 1)  

