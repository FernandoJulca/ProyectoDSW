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

create or alter proc sp_Distrito
@tipo varchar(50),
@id int
as
begin
	if @tipo= 'distritoId'
	begin
	select ID_DISTRITO,NOMBRE from TB_DISTRITO where ID_DISTRITO = @id
	end
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
			values (@ruc,@razon,@telefono,@direccion,@idDistrito,@fecha,1);
			select @@IDENTITY
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
			select ID_PROVEEDOR, RUC, RAZON_SOCIAL, TELEFONO, DIRECCION,d.ID_DISTRITO ,d.NOMBRE, FECHA_REGISTRO, ESTADO
			from TB_PROVEEDOR p
			Inner join TB_DISTRITO d on p.ID_DISTRITO = d.ID_DISTRITO 
			where ID_PROVEEDOR = @idProveedor
		end

	if @tipo = 'desactivar'
		begin
			update TB_PROVEEDOR 
			set ESTADO = 0 where ID_PROVEEDOR = @idProveedor
		end
end
GO
select * from TB_DISTRITO
go

select * From TB_PROVEEDOR where ID_PROVEEDOR = 21
go

select * From TB_PRODUCTO
go

--SCRIPTS PRODUCTOS
create or alter proc listarProductos
as 
begin
	select pr.ID_PRODUCTO, pr.NOMBRE, pr.DESCRIPCION, pv.RAZON_SOCIAL, ct.DESCRIPCION, pr.PRECIO, pr.STOCK from TB_PRODUCTO pr 
	INNER JOIN TB_CATEGORIA ct ON pr.ID_CATEGORIA = ct.ID_CATEGORIA
	INNER JOIN TB_PROVEEDOR pv ON pr.ID_PROVEEDOR = pv.ID_PROVEEDOR where pr.ESTADO = 1
end
go

create or alter proc crudProductos
@tipo varchar(50) = '',
@idProducto int = 0,
@nombre varchar(50) = '',
@descripcion varchar(100) = '',
@idProveedor int = 0,
@idCategoria int = 0,
@precio decimal(10,2) =0.0,
@stock int = 0,
@imagen varchar(100) = '',
@fecha datetime = null,
@estado bit = 1  
as
begin
	
	if @tipo = 'registrar'
		begin
			insert into TB_PRODUCTO(NOMBRE, DESCRIPCION, ID_PROVEEDOR, ID_CATEGORIA, PRECIO, STOCK, IMAGEN, FECHA_REGISTRO, ESTADO)
			values(@nombre,@descripcion,@idProveedor,@idCategoria,@precio,@stock,@imagen,@fecha,1)
			select @@IDENTITY
		end	
	
	if @tipo = 'actualizar'
		begin
			if @imagen is Null
				update TB_PRODUCTO 
					set NOMBRE = @nombre, DESCRIPCION = @descripcion,ID_PROVEEDOR=@idProveedor,ID_CATEGORIA = @idCategoria,
					PRECIO = @precio, STOCK = @stock, FECHA_REGISTRO = @fecha, ESTADO = 1 where ID_PRODUCTO = @idProducto
			else
				update TB_PRODUCTO 
					set NOMBRE = @nombre, DESCRIPCION = @descripcion,ID_PROVEEDOR=@idProveedor,ID_CATEGORIA = @idCategoria,
					PRECIO = @precio, STOCK = @stock,IMAGEN = @imagen, FECHA_REGISTRO = @fecha, ESTADO = 1 where ID_PRODUCTO = @idProducto
		end
	if @tipo = 'desactivar'
		begin
			update TB_PRODUCTO set ESTADO = 0 where ID_PRODUCTO = @idProducto
		end

	if @tipo = 'detalle'
		begin
			select pr.ID_PRODUCTO, pr.NOMBRE, pr.DESCRIPCION,pv.ID_PROVEEDOR ,pv.RAZON_SOCIAL,ct.ID_CATEGORIA ,ct.DESCRIPCION, pr.PRECIO, pr.STOCK, pr.IMAGEN,
				   pr.FECHA_REGISTRO, pr.ESTADO		
			from TB_PRODUCTO pr 
			INNER JOIN TB_CATEGORIA ct ON pr.ID_CATEGORIA = ct.ID_CATEGORIA
			INNER JOIN TB_PROVEEDOR pv ON pr.ID_PROVEEDOR = pv.ID_PROVEEDOR where ID_PRODUCTO = @idProducto
		end

end
go

exec crudProductos 'detalle',54

select * from  TB_PRODUCTO