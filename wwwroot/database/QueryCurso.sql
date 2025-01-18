-- Crear base de datos
CREATE DATABASE ManagementCurso

-- Uso de la base de datos
USE ManagementCurso

-- Eliminacion de la base de datos
DROP DATABASE ManagementCurso;

-- Crear tabla tipo operacion
CREATE TABLE Tipo_Operacion (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Descripcion VARCHAR(500),
	Create_at DATE DEFAULT GETDATE()
);

-- Crear tabla usuario
CREATE TABLE Usuario(
	Id INT Primary KEY IDENTITY(1,1),
	Email VARCHAR(300) NOT NULL,
	EmailNormalizado VARCHAR(300) NOT NULL,
	PasswordHash VARCHAR(MAX),
	Create_at DATE DEFAULT GETDATE()
);

-- Crear tabla tipo cuenta
CREATE TABLE Tipo_Cuenta (
	Id INT PRIMARY KEY IDENTITY(1,1),
	IdUsuario INT NOT NULL,
	Nombre VARCHAR(50) NOT NULL,
	Orden INT NOT NULL,
	Create_at DATE DEFAULT GETDATE(),
	-- Clave foránea hacia usuario
	CONSTRAINT FK_TipoCuenta_Usuario FOREIGN KEY (IdUsuario)
	REFERENCES Usuario(Id),
);

-- Crear tabla Cuenta
CREATE TABLE Cuenta(
	Id INT PRIMARY KEY IDENTITY(1,1),
	IdTipoCuenta INT NOT NULL, 
	Nombre VARCHAR (50) NOT NULL,
	Balance DECIMAL (18,2) NOT NULL,
	Descripcion VARCHAR(1000),
	Create_at DATE DEFAULT GETDATE(),
	-- Clave foránea hacia tipo cuenta
	CONSTRAINT FK_Cuenta_TipoCuenta FOREIGN KEY (IdTipoCuenta) 
	REFERENCES Tipo_Cuenta(Id)
);

-- Crear tabla categoria
CREATE TABLE Categoria (
	Id INT PRIMARY KEY IDENTITY(1,1),
	IdUsuario INT NOT NULL,
	IdTipoOperacion INT NOT NULL,
	Nombre VARCHAR(100) NOT NULL,
	Create_at DATE DEFAULT GETDATE()
	-- Clave foránea hacia Usuario
	CONSTRAINT FK_Categoria_Usuario FOREIGN KEY (IdUsuario)
	REFERENCES Usuario(Id),
	-- Clave foránea hacia Tipo operacion
	CONSTRAINT FK_Categoria_TipoOperacion FOREIGN KEY (IdTipoOperacion)
	REFERENCES Tipo_Operacion(Id)
);

-- Crear tabla transacciones
CREATE TABLE Transacciones (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT NOT NULL,
	IdCuenta INT NOT NULL,
	IdCategoria INT NOT NULL,
    Nota VARCHAR(1000),
    Monto DECIMAL(18,2) NOT NULL,
	FechaTransaccion DATE NOT NULL,
    Create_at DATETIME DEFAULT GETDATE(),
	--Clave foránea hacia usuario
	CONSTRAINT Fk_Transacciones_Usuario FOREIGN KEY (IdUsuario)
	REFERENCES Usuario(Id),
	-- Clave foránea hacia cuenta
    CONSTRAINT FK_Transacciones_Cuenta FOREIGN KEY (IdCuenta)
    REFERENCES Cuenta(Id),
	-- Clave foránea hacia categoria
    CONSTRAINT FK_Transacciones_Categoria FOREIGN KEY (IdCategoria)
    REFERENCES Categoria(Id)

);

--Alter table a transacciones para agregar Fecha Transaccion
ALTER TABLE transacciones
ADD FechaTransaccion DATETIME;

-- Query con INNER JOIN de las dos tablas
SELECT * FROM Transacciones
INNER JOIN
Tipo_Operacion ON Transacciones.IdTipoOperacion = Tipo_Operacion.Id;

-- Stored Procedure encapsulando la Query anterior
CREATE PROCEDURE sp_ObtenerTransaccionesConTipo
AS
BEGIN
-- Consulta para obtener las transacciones junto con su tipo
    SELECT 
        Transacciones.Id AS IdTransaccion,
        Transacciones.IdUsuario AS IdUsuario,
        Transacciones.IdTipoOperacion AS IdTipoOperacion,
        Transacciones.Nota AS Nota,
        Transacciones.Monto AS Monto,
        Transacciones.Create_at AS FechaTransaccion,
        Tipo_Operacion.Descripcion AS DescripcionTipoTransaccion,
		Tipo_Operacion.Create_at AS FechaTipoOperacion
    FROM 
        Transacciones
    INNER JOIN 
        Tipo_Operacion ON Transacciones.IdTipoOperacion = Tipo_Operacion.Id;
END;

-- Ejecutar Stored Procedure
EXEC sp_ObtenerTransaccionesConTipo;

-- Stored Procedure encapsulando un proceso de ordenamiento de tabla
CREATE PROCEDURE sp_TiposCuentas_Insertar
	@Nombre VARCHAR(50),
	@IdUsuario INT
AS
BEGIN
		SET NOCOUNT ON;
		DECLARE @Orden INT;
		SELECT @Orden = COALESCE(MAX(Orden),0)+1
		FROM Tipo_Cuenta
		WHERE IdUsuario = @IdUsuario

		INSERT INTO Tipo_Cuenta (Nombre, IdUsuario, Orden) VALUES (@Nombre, @IdUsuario, @Orden) SELECT SCOPE_IDENTITY();
END;

-- Stored Procedure encapsulando un proceso de creacion de transaccion
CREATE PROCEDURE sp_Transacciones_Insertar
	@IdUsuario INT,
	@IdCuenta INT,
	@IdCategoria INT,
	@Nota VARCHAR(1000) = NULL,
	@Monto DECIMAL(18,2),
	@FechaTransaccion DATE
AS
BEGIN
		SET NOCOUNT ON;
		DECLARE @Orden INT;

		INSERT INTO Transacciones (IdUsuario, IdCuenta, IdCategoria, Nota, Monto,FechaTransaccion) VALUES (@IdUsuario, @IdCuenta, @IdCategoria, @Nota, ABS(@Monto), @FechaTransaccion) 
		
		UPDATE Cuenta SET Balance += @Monto WHERE Id = @IdCuenta;

		SELECT SCOPE_IDENTITY();
END;

SELECT * FROM Transacciones;

-- Stored Procedure encapsulando un proceso de edicion de transaccion
CREATE PROCEDURE sp_Transacciones_Actualizar
	@Id INT,
	@IdCuenta INT,
	@IdCuentaAnterior INT,
	@IdCategoria INT,
	@Nota VARCHAR(1000) = NULL,
	@Monto DECIMAL(18,2),
	@MontoAnterior DECIMAL(18,2),
	@FechaTransaccion DATE
AS
BEGIN
		SET NOCOUNT ON;
		DECLARE @Orden INT;
		
		--Revertir transaccion anterior
		UPDATE Cuenta
		SET Balance -=@MontoAnterior
		WHERE Id = @IdCuentaAnterior

		UPDATE Cuenta
		SET Balance += @Monto
		WHERE Id = @IdCuenta

		UPDATE Transacciones
		SET Monto = ABS(@Monto), FechaTransaccion = @FechaTransaccion, IdCategoria = @IdCategoria, IdCuenta = @IdCuenta, Nota = @Nota
		WHERE Id = @Id
END;


-- Stored Procedure encapsulando un proceso de edicion de transaccion
CREATE PROCEDURE sp_Transacciones_Eliminar
	@Id INT
AS
BEGIN
		SET NOCOUNT ON;
		DECLARE @Monto DECIMAL(18,2);
		DECLARE @IdCuenta INT;
		DECLARE @IdTipoOperacion INT;
		
		SELECT @Monto = Monto, @IdCuenta = IdCuenta, @IdTipoOperacion = cat.IdTipoOperacion
		FROM Transacciones 
		INNER JOIN Categoria cat
		ON cat.Id = Transacciones.IdCategoria 
		WHERE Transacciones.Id = @Id;

		DECLARE @FactorMulti INT = 1;

		IF(@IdTipoOperacion = 2)
			SET @FactorMulti = -1;
		SET @Monto = @Monto * @FactorMulti;
		UPDATE Cuenta
		SET Balance -= @Monto
		WHERE Id = @IdCuenta;
		DELETE Transacciones WHERE Id = @Id
END;

