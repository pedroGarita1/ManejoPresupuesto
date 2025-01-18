-- Querys utilizadas para Tipo de Cuenta
SELECT * FROM Tipo_Cuenta;

SELECT 1 FROM Tipo_Cuenta WHERE Nombre = @Nombre AND IdUsuario = @IdUsuario;

SELECT Id, IdUsuario, Nombre, Orden, Create_at FROM Tipo_Cuenta WHERE IdUsuario = @IdUsuario

UPDATE Tipo_Cuenta SET Nombre = @Nombre WHERE Id = @Id

SELECT Id, IdUsuario, Nombre, Orden, Create_at FROM Tipo_Cuenta WHERE Id = @Id AND IdUsuario = @IdUsuario

DELETE Tipo_Cuenta WHERE Id = @Id
UPDATE Tipo_Cuenta SET ORDEN = @Orden Where Id = @Id

-- Querys utilizadas para Cuenta
SELECT * FROM Cuenta
INSERT INTO Cuenta (IdTipoCuenta, Nombre, Descripcion, Balance) VALUES (@IdTipoCuenta, Nombre, Descripcion, Balance);

SELECT c.Id, c.Nombre, c.Balance, c.Create_at, tc.Nombre AS TipoCuenta
FROM Cuenta AS c 
INNER JOIN Tipo_Cuenta AS tc ON tc.Id = c.IdTipoCuenta 
WHERE tc.IdUsuario = @IdUsuario ORDER BY tc.Orden

UPDATE Cuenta SET Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion, IdTipoCuenta = @IdTipoCuenta WHERE Id = @Id

DELETE Cuenta WHERE Id = @Id

-- Querys Utilizadas para Categorias
SELECT * FROM Categoria;
INSERT INTO Categoria (idUsuario, IdTipoOperacion, Nombre) VALUES (@idUsuario, @IdTipoOperacion, @Nombre)

UPDATE Categoria SET IdTipoOperacion = @IdTipoOperacion, Nombre = @Nombre WHERE Id = @Id

DELETE Categoria WHERE Id = @Id

-- Querys Utilizadas para Tipo Operaciones
SELECT * FROM Tipo_Operacion

-- Query utilizadas para Tipo Transacciones
SELECT * FROM Transacciones;

SELECT t.*, cat.IdTipoOperacion FROM Transacciones t INNER JOIN Categoria cat ON cat.Id = t.IdCategoria WHERE t.Id = @Id AND t.IdUsuario = @IdUsuario

SELECT t.Id, t.Monto, t.FechaTransaccion, c.Nombre AS Categoria, cu.Nombre AS Cuenta, c.IdTipoOperacion
FROM Transacciones t 
INNER JOIN Categoria c ON c.Id = t.IdCategoria 
INNER JOIN Cuenta cu ON cu.Id = t.IdCuenta 
WHERE t.IdCuenta = @IdCuenta 
AND t.IdUsuario = @IdUsuario 
AND t.FechaTransaccion BETWEEN @FechaInicio AND @FechaFin

SELECT t.Id, t.Monto, t.FechaTransaccion, c.Nombre AS Categoria, cu.Nombre AS Cuenta, c.IdTipoOperacion
FROM Transacciones t 
INNER JOIN Categoria c ON c.Id = t.IdCategoria 
INNER JOIN Cuenta cu ON cu.Id = t.IdCuenta 
WHERE t.IdUsuario = @IdUsuario 
AND t.FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
ORDER BY t.FechaTransaccion DESC