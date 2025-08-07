/*SCRIPTS TOOLIFY*/
USE BD_TOOLIFY
GO

create or alter proc listarProveedores
as
begin
	SELECT pv.ID_PROVEEDOR, pv.RUC, pv.RAZON_SOCIAL, pv.TELEFONO, 
	pv.DIRECCION, dt.NOMBRE, pv.FECHA_REGISTRO, pv.ESTADO FROM TB_PROVEEDOR pv
	INNER JOIN TB_DISTRITO dt ON pv.ID_DISTRITO = dt.ID_DISTRITO where pv.ESTADO = 1
end
go

create or alter proc crudProveedores
@tipo varchar(50),
@idProveedor int = 0,
@ruc char(11) = '',
@razon varchar(100) = '',
@telefono char(15) = '',
@direccion varchar(80) = '',
@idDistrito int = 0,
@fecha datetime = null,
@estado bit = 1
as 
begin
	if @tipo = 'registrar'
		begin
			insert into TB_PROVEEDOR(RUC, RAZON_SOCIAL, TELEFONO, DIRECCION, ID_DISTRITO, FECHA_REGISTRO,ESTADO)
			values (@ruc,@razon,@telefono,@direccion,@idDistrito,@fecha,1)
		end
	
	if @tipo = 'actualizar'
		begin
			update TB_PROVEEDOR 
			set RUC = @ruc, RAZON_SOCIAL = @razon,TELEFONO= @telefono,DIRECCION = @direccion,
			ID_DISTRITO=@idDistrito,FECHA_REGISTRO = @fecha ,estado = 1
			where ID_PROVEEDOR = @idProveedor
		end
	
	if @tipo = 'detalle'
		begin
			select ID_PROVEEDOR, RUC, RAZON_SOCIAL, TELEFONO, DIRECCION, ID_DISTRITO, FECHA_REGISTRO, ESTADO
			from TB_PROVEEDOR where ID_PROVEEDOR = @idProveedor
		end

	if @tipo = 'desactivar'
		begin
			update TB_PROVEEDOR 
			set ESTADO = 0 where ID_PROVEEDOR = @idProveedor
		end
end
