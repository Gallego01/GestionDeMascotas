**Descripcion breve**

Proyecto para administrar mascotas desde consola. Permite cargar, modificar, borrar y listar mascotas (Perro, Gato y Otros). Usa SQL Server para guardar datos y ADO.NET para las consultas.
Incluye manejo de datos especificos segun tipo de mascota y esquema de vacunacion segun edad.

**Tecnologias Usadas**
  +	C# .NET
  +	SQL Server
  +	ADO.NET para conexion y comandos SQL
  +	Programacion orientada a objetos (POO)
  +	Arquitectura basada en servicios (MascotaService, PerroService, GatoService)
  +	Uso de Guard Clauses para validacion

**Funcionamiento**

*Cargar Mascota*
  +	Se ingresa nombre, especie, sexo, peso y fecha de nacimiento (opcional fecha aleatoria).
  +	Si es Perro o Gato, se piden datos especificos (raza, color, vacunado).
  +	Se guarda en la base de datos las tablas “Mascota” y segun corresponda, “Perro” o “Gato”.

*Mostrar Mascota*
  +	Se selecciona tres opciones (Perro, Gato, Otros, Todos)
  +	Se muestra un listado correspondiente a lo seleccionado anteriormente

*Modificar Mascota*
  +	Se busca mascota por nombre o parte del nombre.
  +	Se muestra listado si hay multiples resultados para seleccionar.
  +	Se puede modificar cualquier campo basico o especifico de la mascota.

*Borrar Mascota*
  +	Se busca mascota por nombre o parte del nombre.
  +	Se confirma eliminacion y se borran registros relacionados.

*Listar Mascotas Vacunables*
  +	Muestra mascotas que cumplen el esquema de vacunacion segun edad (en meses).
  +	El esquema incluye vacunas multiples, refuerzos anuales, etc.

*Crear Archivo .TXT (Serialize)*
  +	Se selecciona una de las tres opciones (Perro, Gato, Otros, Todos)
  +	Crear el archivo correspondiente seleccionado el cual se guarda en la carpeta del Proyecto
  +	(GestionDeMascotas\GestionDeMascotas\bin\Debug)

*Lectura de Archivo .TXT (Deserializer)*
  +	Se selecciona una de las tres opciones (Perro, Gato, Otros, Todos)
  +	Muestra por consola los datos de la opcion seleccionada

**Estructura de Codigo**
  +	MascotaService: servicio principal para CRUD de mascotas
  +	PerroService y GatoService: servicios para manejar datos especificos de perros y gatos
  +	GuardClause: validacion de entrada y utilidades como calculo de edad, menus
  +	Models: clases Mascota, Perro, Gato con propiedades necesarias
  +	Conexion SQL: gestionada con mascotaContext.GetConnection() que devuelve SqlConnection

**Uso**
1.	Clonar repo
2.	Configurar cadena de conexion en MascotaContext
3.	Ejecutar SQL para crear tablas: (Mascota, Perro, Gato)
4.	Compilar y ejecutar programa consola
5.	Seguir menu para cargar, mostrar, modificar, borrar , listar mascotas(por consola),
    Serializar lista de mascotas correspondientes(gato, perro, otros), lectura de serializacion de la misma

**Base de Datos**
```sql
CREATE TABLE Mascota (
    id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(50),
    especie VARCHAR(50),
    sexo VARCHAR(50),
    peso FLOAT,
    fechaNacimiento DATETIME,
    edadEnMeses int
);
	
CREATE TABLE Perro (
    mascotaId INT PRIMARY KEY,
    raza VARCHAR(50),
    color VARCHAR(50),
    vacunado BIT
);
	
CREATE TABLE Gato (
    mascotaId INT PRIMARY KEY,
    raza VARCHAR(50),
    color VARCHAR(50),
    vacunado BIT
);
	
ALTER TABLE Perro
ADD CONSTRAINT FK_Perro_Mascota
FOREIGN KEY (mascotaId) REFERENCES Mascota(id);

ALTER TABLE Gato
ADD CONSTRAINT FK_Gato_Mascota
FOREIGN KEY (mascotaId) REFERENCES Mascota(id);
```
**Tablas principales**
  +	Mascota: id, nombre, especie, sexo, peso, fechaNacimiento, edadEnMeses
  +	Perro: mascotaId (FK), raza, color, vacunado
  +	Gato: mascotaId (FK), raza, color, vacunado

**Relaciones**
  +	Perro.mascotaId y Gato.mascotaId FK a Mascota.id
