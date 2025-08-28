﻿USE master
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'BD_TOOLIFY')
BEGIN
    ALTER DATABASE [BD_TOOLIFY] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [BD_TOOLIFY];
END
GO

CREATE DATABASE [BD_TOOLIFY];
GO

USE [BD_TOOLIFY];
GO

CREATE TABLE TB_DISTRITO (
    ID_DISTRITO INT PRIMARY KEY IDENTITY(1,1),
    NOMBRE VARCHAR(50) NOT NULL
);

CREATE TABLE TB_CATEGORIA (
    ID_CATEGORIA INT PRIMARY KEY IDENTITY(1,1),
    DESCRIPCION VARCHAR(25) NOT NULL
);

CREATE TABLE TB_ROL (
    ID_ROL INT PRIMARY KEY IDENTITY(1,1),
    DESCRIPCION CHAR(1) NOT NULL
);

CREATE TABLE TB_USUARIO (
    ID_USUARIO INT PRIMARY KEY IDENTITY(1,1),
    NOMBRES VARCHAR(50) NOT NULL,
    APE_MATERNO VARCHAR(50) NOT NULL,
    APE_PATERNO VARCHAR(50) NOT NULL,
    CORREO VARCHAR(50) UNIQUE NOT NULL,
    CLAVE VARCHAR(225) NOT NULL,
    NRO_DOC VARCHAR(15) UNIQUE NOT NULL,
    DIRECCION VARCHAR(50) NULL,
    ID_DISTRITO INT NOT NULL,
    TELEFONO CHAR(9) NOT NULL,
    ROL INT DEFAULT 2,
    FECHA_REGISTRO DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ID_DISTRITO) REFERENCES TB_DISTRITO(ID_DISTRITO),
    FOREIGN KEY (ROL) REFERENCES TB_ROL(ID_ROL)
);

CREATE TABLE TB_PROVEEDOR (
    ID_PROVEEDOR INT PRIMARY KEY IDENTITY(1,1),
    RUC CHAR(11) UNIQUE NOT NULL, 
    RAZON_SOCIAL VARCHAR(100) NOT NULL,
    TELEFONO CHAR(15) NOT NULL,
    DIRECCION VARCHAR(80) NOT NULL,
    ID_DISTRITO INT NOT NULL,
    FECHA_REGISTRO DATETIME DEFAULT GETDATE(),
    ESTADO BIT DEFAULT 1,
    FOREIGN KEY (ID_DISTRITO) REFERENCES TB_DISTRITO(ID_DISTRITO)
);

CREATE TABLE TB_PRODUCTO (
    ID_PRODUCTO INT PRIMARY KEY IDENTITY(1,1),
    NOMBRE VARCHAR(50) NOT NULL,
    DESCRIPCION VARCHAR(100) NOT NULL,
    ID_PROVEEDOR INT,
    ID_CATEGORIA INT,
    PRECIO DECIMAL(10,2) NOT NULL,
    STOCK INT DEFAULT 1 NOT NULL,
    IMAGEN varchar(MAX) NULL,
    FECHA_REGISTRO DATETIME DEFAULT GETDATE(),
    ESTADO BIT DEFAULT 1,
    FOREIGN KEY (ID_PROVEEDOR) REFERENCES TB_PROVEEDOR(ID_PROVEEDOR),
    FOREIGN KEY (ID_CATEGORIA) REFERENCES TB_CATEGORIA(ID_CATEGORIA)
);

CREATE TABLE TB_VENTA (
    ID_VENTA INT PRIMARY KEY IDENTITY(1,1),
    ID_USUARIO INT,
    FECHA DATETIME DEFAULT GETDATE(),
    TOTAL DECIMAL(11,2),
    ESTADO CHAR(1),
    CONSTRAINT EST_CHECK CHECK (ESTADO IN ('G','P','C','T', 'E')), -- Generado, Pendiente, Cancelado
    TIPO_VENTA CHAR(1) DEFAULT 'P', -- 'P' (Presencial), 'R' (Remota)
    FOREIGN KEY (ID_USUARIO) REFERENCES TB_USUARIO(ID_USUARIO)
);

CREATE TABLE TB_DETALLE_VENTA (
    ID_DETALLE_VENTA INT PRIMARY KEY IDENTITY(1,1),
    ID_VENTA INT NOT NULL,
    ID_PRODUCTO INT NOT NULL,
    CANTIDAD SMALLINT NOT NULL,
    SUB_TOTAL DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (ID_VENTA) REFERENCES TB_VENTA(ID_VENTA),
    FOREIGN KEY (ID_PRODUCTO) REFERENCES TB_PRODUCTO(ID_PRODUCTO)
);


-- INSERCCIONES

INSERT INTO TB_DISTRITO (NOMBRE) VALUES
('Ancón'),
('Ate'),
('Barranco'),
('Breña'),
('Carabayllo'),
('Chorrillos'),
('Comas'),
('La Molina'),
('Miraflores'),
('San Isidro');

INSERT INTO TB_CATEGORIA (DESCRIPCION) VALUES
('Drywall'),
('Fierro'),
('Puertas'),
('Taladros'),
('Compresoras'),
('Cables eléctricos'),
('Inodoros'),
('Tanques agua'),
('Porcelanatos'),
('Pisos cerámicos');

INSERT INTO TB_ROL (DESCRIPCION) VALUES
('A'), -- Administrador
('C'), -- Cliente
('V'), -- Vendedor
('R'); -- Repartidor


INSERT INTO TB_USUARIO (NOMBRES, APE_PATERNO, APE_MATERNO, CORREO, CLAVE, NRO_DOC, DIRECCION, ID_DISTRITO, TELEFONO, ROL)
VALUES
('Luis','Pérez','García','luis.perez@example.com','clave123','12345678','Av. Lima 123',1,'987654321',1),
('María','Ramírez','Lopez','maria.ramirez@example.com','clave123','23456789','Calle Real 456',2,'987654322',2),
('Juan','Gonzales','Meza','juan.gonzales@example.com','clave123','34567890','Jr. Perú 789',3,'987654323',3),
('Ana','Torres','Díaz','ana.torres@example.com','clave123','45678901','Psje. Andino 012',4,'987654324',4);

INSERT INTO TB_PROVEEDOR (RUC, RAZON_SOCIAL, TELEFONO, DIRECCION, ID_DISTRITO)
VALUES
('20123456781','Ferretería El Maestro','014567890','Av. Constructores 123',1),
('20123456782','Construye Perú SAC','014567891','Jr. Obrero 456',2),
('20123456783','Materiales Lima SAC','014567892','Calle Industria 789',3),
('20123456784','Innova Construcción','014567893','Av. Diseño 101',4),
('20123456785','Proveedora Andina','014567894','Psje. Andino 202',5),
('20123456786','Equipamientos SAC','014567895','Calle Equipos 303',6),
('20123456787','Ferreval SAC','014567896','Av. Ferretería 404',7),
('20123456788','Metalúrgica Perú','014567897','Jr. Metal 505',8),
('20123456789','Cerámicos del Sur','014567898','Av. Sur 606',9),
('20123456790','Porcelanatos Lima','014567899','Calle Porcelana 707',10),
('20123456791','Soluciones Hidráulicas','014567800','Av. Agua 808',1),
('20123456792','Tanques & Cía','014567801','Jr. Tanque 909',2),
('20123456793','Electric Perú','014567802','Calle Luz 010',3),
('20123456794','Cables y Más SAC','014567803','Av. Cable 111',4),
('20123456795','Puertas Seguras SAC','014567804','Psje. Seguridad 222',5),
('20123456796','Taladros Perú','014567805','Calle Taladro 333',6),
('20123456797','Compresores SAC','014567806','Av. Compresor 444',7),
('20123456798','Drywall House','014567807','Jr. Drywall 555',8),
('20123456799','Fierro y Acero SAC','014567808','Av. Fierro 666',9),
('20123456800','Constructora Integral','014567809','Calle Construir 777',10);

INSERT INTO TB_PRODUCTO (NOMBRE, DESCRIPCION, ID_PROVEEDOR, ID_CATEGORIA, PRECIO, STOCK, IMAGEN) VALUES
('Panel Drywall 12mm', 'Panel estándar 1.20x2.44m', 1, 1, 45.50, 100, NULL),
('Panel Drywall 15mm', 'Panel reforzado 1.20x2.44m', 1, 1, 55.00, 80, NULL),
('Perfil U 3m', 'Perfil metálico U para drywall', 2, 1, 12.00, 200, NULL),
('Perfil C 3m', 'Perfil metálico C para drywall', 2, 1, 15.00, 150, NULL),
('Puerta Madera 0.80x2.00', 'Puerta interior MDF', 15, 3, 250.00, 50, NULL),
('Puerta Metal 0.90x2.00', 'Puerta metal reforzada', 15, 3, 350.00, 40, NULL),
('Taladro Percutor 650W', 'Taladro con martillo y brocas', 16, 4, 180.00, 60, NULL),
('Taladro Inalámbrico 18V', 'Taladro inalámbrico con batería', 16, 4, 440.00, 30, NULL),
('Compresor 50L', 'Compresor de aire 2 HP', 17, 5, 720.00, 25, NULL),
('Compresor 100L', 'Compresor industrial 3 HP', 17, 5, 920.00, 15, NULL),
('Cable Eléctrico 2.5mm²', 'Rollo cable cobre', 13, 6, 0.80, 500, NULL),
('Cable Eléctrico 4mm²', 'Rollo cable cobre', 13, 6, 1.20, 400, NULL),
('Inodoro Línea Premium', 'Inodoro cerámico con tapa', 9, 7, 360.00, 70, NULL),
('Inodoro Compacto', 'Inodoro cerámico compacto', 9, 7, 240.00, 90, NULL),
('Tanque Agua 300L', 'Tanque plástico vertical', 11, 8, 450.00, 35, NULL),
('Tanque Agua 500L', 'Tanque plástico vertical', 11, 8, 650.00, 20, NULL),
('Porcelanato 60x60cm', 'Porcelanato brillo gris', 10, 9, 75.00, 120, NULL),
('Porcelanato 80x80cm', 'Porcelanato satinado beige', 10, 9, 95.00, 80, NULL),
('Piso Cerámico 30x30cm', 'Cerámica interior lisa', 10, 10, 35.00, 150, NULL),
('Piso Cerámico 45x45cm', 'Cerámica interior diseño', 10, 10, 50.00, 100, NULL),
('Drywall Resistente a Humedad', 'Panel verde 12mm', 18, 1, 55.00, 90, NULL),
('Perfil Drywall Galva', 'Perfil galvanizado U 2.5m', 18, 1, 13.00, 180, NULL),
('Puerta de Seguridad', 'Puerta exterior acero', 15, 3, 980.00, 30, NULL),
('Puerta Corrediza 2.00m', 'Puerta corrediza madera', 15, 3, 420.00, 45, NULL),
('Taladro SDS 750W', 'Taladro SDS pro', 16, 4, 520.00, 35, NULL),
('Juego Brocas SDS', 'Set 10 brocas SDS', 16, 4, 75.00, 120, NULL),
('Mini Compresor Portátil', 'Compresor 10L portátil', 17, 5, 320.00, 40, NULL),
('Manguera 20m', 'Manguera para compresor', 17, 5, 45.00, 100, NULL),
('Cable Subterráneo 6mm²', 'Cable enrollable', 13, 6, 2.50, 300, NULL),
('Cable Subterráneo 10mm²', 'Cable enrollable', 13, 6, 4.00, 200, NULL),
('Inodoro Ducha Bidet', 'Inodoro con bidet', 9, 7, 480.00, 60, NULL),
('Lavatricero Completo', 'Lavatorio y mueble', 9, 7, 560.00, 40, NULL),
('Tanque Agua Horizontal 200L', 'Tanque plástico horizontal', 11, 8, 380.00, 30, NULL),
('Tanque Agua Horizontal 400L', 'Tanque plástico horizontal', 11, 8, 580.00, 20, NULL),
('Porcelanato Imitación Madera', 'Porcelanato 20x120cm', 10, 9, 120.00, 60, NULL),
('Porcelanato Imitación Mármol', 'Porcelanato 60x60cm', 10, 9, 140.00, 50, NULL),
('Piso Cerámico Antideslizante', 'Piso baño 30x30cm', 10, 10, 45.00, 130, NULL),
('Piso Cerámico Exterior', 'Piso rústico 30x30cm', 10, 10, 55.00, 110, NULL),
('Panel Drywall Fuego 12mm', 'Panel resistente fuego', 18, 1, 60.00, 70, NULL),
('Perfil F019 3m', 'Perfil galvanizado para terminación', 18, 1, 14.00, 160, NULL),
('Taladro Atornillador 14V', 'Taladro compacto', 16, 4, 320.00, 50, NULL),
('Taladro De Impacto 600W', 'Taladro compacto impacto', 16, 4, 210.00, 65, NULL),
('Compresor Industrial 150L', 'Compresor 5 HP', 17, 5, 1250.00, 10, NULL),
('Compresor Silencioso 24L', 'Compresor para pintura', 17, 5, 540.00, 25, NULL),
('Cable Flex 16mm²', 'Cable flexible enrollado', 13, 6, 6.50, 180, NULL),
('Cable Flex 25mm²', 'Cable flexible enrollado', 13, 6, 10.00, 120, NULL),
('Inodoro Smart Dual Flush', 'Inodoro doble descarga', 9, 7, 720.00, 50, NULL),
('Inodoro One Piece', 'Inodoro cerámico compacto', 9, 7, 480.00, 70, NULL),
('Tanque Agua Pressurizado 120L', 'Tanque con bomba', 11, 8, 780.00, 15, NULL),
('Tanque Agua Pressurizado 220L', 'Tanque con bomba', 11, 8, 980.00, 12, NULL),
('Porcelanato Gris Mate 60x120cm', 'Porcelanato gris mate', 10, 9, 160.00, 40, NULL),
('Porcelanato Blanco Brillo', 'Porcelanato blanco brillo 60x60', 10, 9, 150.00, 45, NULL),
('Piso Cerámico 60x60cm', 'Cerámica interior lisa', 10, 10, 65.00, 100, NULL);

--datos prueba para las ventas 

INSERT INTO TB_USUARIO 
(NOMBRES, APE_PATERNO, APE_MATERNO, CORREO, CLAVE, NRO_DOC, DIRECCION, ID_DISTRITO, TELEFONO, ROL)
VALUES
('Lucía', 'Morales', 'Castillo', 'lucia.morales@gmail.com', 'abc123', '33445566', 'Av. San Martín 101', 1, '921345678', 2),
('Diego', 'Suárez', 'Rojas', 'diego.suarez@gmail.com', 'abc123', '44556677', 'Calle Central 202', 2, '934567890', 2),
('Andrea', 'Vega', 'Campos', 'andrea.vega@gmail.com', 'abc123', '55667788', 'Jr. Primavera 303', 3, '945678901', 2);

select * from TB_USUARIO where ROL = 2
go

INSERT INTO TB_VENTA (ID_USUARIO, FECHA, TOTAL, ESTADO, TIPO_VENTA)
VALUES
-- ENERO (3 registros)
(2, '2025-01-12', 220.50, 'P', 'P'),
(5, '2025-01-25', 310.75, 'G', 'R'),
(6, '2025-01-30', 180.00, 'C', 'P'),

-- FEBRERO (3 registros)
(7, '2025-02-05', 450.20, 'P', 'R'),
(2, '2025-02-14', 390.00, 'G', 'P'),
(5, '2025-02-28', 600.10, 'C', 'R'),

-- MARZO (4 registros)
(6, '2025-03-03', 270.50, 'G', 'P'),
(7, '2025-03-11', 150.00, 'P', 'R'),
(2, '2025-03-20', 340.40, 'C', 'P'),
(5, '2025-03-27', 710.30, 'P', 'R'),

-- ABRIL (7 registros)
(6, '2025-04-02', 560.15, 'G', 'P'),
(7, '2025-04-05', 890.00, 'P', 'R'),
(2, '2025-04-10', 420.75, 'C', 'P'),
(5, '2025-04-14', 310.20, 'G', 'R'),
(6, '2025-04-18', 799.90, 'P', 'P'),
(7, '2025-04-22', 260.40, 'C', 'R'),
(2, '2025-04-28', 675.50, 'G', 'P'),

-- MAYO (7 registros)
(5, '2025-05-03', 500.25, 'P', 'P'),
(6, '2025-05-07', 910.10, 'G', 'R'),
(7, '2025-05-12', 350.00, 'C', 'P'),
(2, '2025-05-15', 470.85, 'P', 'R'),
(5, '2025-05-19', 289.60, 'G', 'P'),
(6, '2025-05-23', 630.00, 'C', 'R'),
(7, '2025-05-28', 780.45, 'P', 'P'),

-- JUNIO (6 registros)
(2, '2025-06-01', 245.90, 'G', 'R'),
(5, '2025-06-06', 550.75, 'P', 'P'),
(6, '2025-06-10', 325.40, 'C', 'R'),
(7, '2025-06-15', 880.00, 'G', 'P'),
(2, '2025-06-20', 710.25, 'P', 'R'),
(5, '2025-06-26', 295.50, 'C', 'P'),

-- JULIO (6 registros)
(6, '2025-07-02', 420.00, 'G', 'P'),
(7, '2025-07-06', 610.30, 'P', 'R'),
(2, '2025-07-11', 370.75, 'C', 'P'),
(5, '2025-07-16', 805.50, 'P', 'R'),
(6, '2025-07-21', 260.20, 'G', 'P'),
(7, '2025-07-29', 910.80, 'C', 'R'),

-- AGOSTO (6 registros)
(2, '2025-08-03', 235.20, 'G', 'P'),
(5, '2025-08-08', 890.10, 'P', 'R'),
(6, '2025-08-13', 430.00, 'C', 'P'),
(7, '2025-08-18', 720.60, 'G', 'R'),
(2, '2025-08-22', 580.25, 'P', 'P'),
(5, '2025-08-27', 315.90, 'C', 'R'),

-- SEPTIEMBRE (3 registros)
(6, '2025-09-05', 220.40, 'G', 'P'),
(7, '2025-09-12', 455.75, 'P', 'R'),
(2, '2025-09-20', 300.00, 'C', 'P'),

-- OCTUBRE (3 registros)
(5, '2025-10-04', 615.90, 'G', 'R'),
(6, '2025-10-11', 280.00, 'P', 'P'),
(7, '2025-10-28', 750.30, 'C', 'R'),

-- NOVIEMBRE (3 registros)
(2, '2025-11-03', 490.20, 'G', 'P'),
(5, '2025-11-15', 350.00, 'P', 'R'),
(6, '2025-11-26', 810.10, 'C', 'P'),

-- DICIEMBRE (9 registros)
(7, '2025-12-01', 999.90, 'G', 'P'),
(2, '2025-12-04', 750.00, 'P', 'R'),
(5, '2025-12-07', 540.25, 'C', 'P'),
(6, '2025-12-10', 860.10, 'G', 'R'),
(7, '2025-12-14', 420.75, 'P', 'P'),
(2, '2025-12-18', 670.40, 'C', 'R'),
(5, '2025-12-22', 930.00, 'P', 'P'),
(6, '2025-12-26', 310.20, 'G', 'R'),
(7, '2025-12-30', 1200.00, 'P', 'P');

