-- Tabla: GRADO_ACADEMICO
-- Propósito: Define los niveles de dificultad base para el juego adaptativo (velocidad y tamaño de palabras).
CREATE TABLE GRADO_ACADEMICO (
    id_grado SERIAL PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL, -- Ej. '1ro Primaria', '2do Primaria'
    velocidad_spawn INT NOT NULL, 
    frecuencia_spawn INT NOT NULL, 
    longitud_max_palabra INT NOT NULL
);

-- Tabla: USUARIO
-- Propósito: Almacena las credenciales y datos de perfil de administradores, profesores y alumnos.
CREATE TABLE USUARIO (
    id_usuario SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    nombre_completo VARCHAR(150),
    password_hash VARCHAR(255) NOT NULL,
    ruta_foto_perfil VARCHAR(255),
    rol VARCHAR(20) NOT NULL, -- Valores permitidos: 'Admin', 'Profesor', 'Alumno'
    activo BOOLEAN DEFAULT TRUE,
    id_grado INT,
    CONSTRAINT fk_grado_usuario FOREIGN KEY (id_grado) REFERENCES GRADO_ACADEMICO(id_grado)
);

-- Tabla: CONFIGURACION
-- Propósito: Guarda las preferencias de interfaz, audio y accesibilidad específicas de cada usuario.
CREATE TABLE CONFIGURACION (
    id_configuracion SERIAL PRIMARY KEY,
    id_usuario INT UNIQUE NOT NULL,
    volumen_master DECIMAL(3,2) DEFAULT 1.0,
    volumen_musica DECIMAL(3,2) DEFAULT 1.0,
    volumen_sfx DECIMAL(3,2) DEFAULT 1.0,
    sonido_teclado BOOLEAN DEFAULT TRUE,
    dislexia_font BOOLEAN DEFAULT FALSE,
    screen_shake BOOLEAN DEFAULT TRUE,
    tamano_meteoritos VARCHAR(20) DEFAULT 'Mediano', -- Valores permitidos: 'Pequeno', 'Mediano', 'Grande'
    modo_diccionario VARCHAR(20) DEFAULT 'Normal', -- Valores permitidos: 'Normal', 'Programador'
    modo_penalizacion VARCHAR(20) DEFAULT 'Flexible', -- Valores permitidos: 'Flexible', 'Estricto'
    mostrar_hud BOOLEAN DEFAULT TRUE,
    CONSTRAINT fk_usuario_config FOREIGN KEY (id_usuario) REFERENCES USUARIO(id_usuario) ON DELETE CASCADE
);

-- Tabla: GRUPO
-- Propósito: Estructura organizativa creada y gestionada por los profesores para agrupar a sus alumnos.
CREATE TABLE GRUPO (
    id_grupo SERIAL PRIMARY KEY,
    id_profesor INT NOT NULL,
    nombre_grupo VARCHAR(50) NOT NULL,
    ciclo_escolar VARCHAR(20),
    CONSTRAINT fk_profesor FOREIGN KEY (id_profesor) REFERENCES USUARIO(id_usuario)
);

-- Tabla: GRUPO_ALUMNO
-- Propósito: Tabla intermedia que resuelve la relación de muchos a muchos entre alumnos y grupos.
CREATE TABLE GRUPO_ALUMNO (
    id_grupo INT NOT NULL,
    id_alumno INT NOT NULL,
    PRIMARY KEY (id_grupo, id_alumno),
    CONSTRAINT fk_grupo FOREIGN KEY (id_grupo) REFERENCES GRUPO(id_grupo) ON DELETE CASCADE,
    CONSTRAINT fk_alumno_grupo FOREIGN KEY (id_alumno) REFERENCES USUARIO(id_usuario) ON DELETE CASCADE
);

-- Tabla: PALABRA
-- Propósito: Diccionario dinámico de palabras que aparecerán en el gameplay, categorizadas por grado.
CREATE TABLE PALABRA (
    id_palabra SERIAL PRIMARY KEY,
    texto VARCHAR(50) UNIQUE NOT NULL,
    categoria VARCHAR(30) DEFAULT 'Normal', -- Valores permitidos: 'Normal', 'Programador'
    id_grado INT,
    CONSTRAINT fk_grado_palabra FOREIGN KEY (id_grado) REFERENCES GRADO_ACADEMICO(id_grado)
);

-- Tabla: PARTIDA
-- Propósito: Historial general de sesiones jugadas para calcular el desempeño y progreso del alumno.
CREATE TABLE PARTIDA (
    id_partida SERIAL PRIMARY KEY,
    id_alumno INT NOT NULL,
    fecha_inicio TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_fin TIMESTAMP,
    puntaje_final INT DEFAULT 0,
    ppm_promedio DECIMAL(5,2) DEFAULT 0.0,
    precision_porcentaje DECIMAL(5,2) DEFAULT 0.0,
    CONSTRAINT fk_alumno_partida FOREIGN KEY (id_alumno) REFERENCES USUARIO(id_usuario)
);

-- Tabla: ERROR_TIPEO
-- Propósito: Registro granular de las palabras exactas que el usuario falló para generar métricas de aprendizaje.
CREATE TABLE ERROR_TIPEO (
    id_error SERIAL PRIMARY KEY,
    id_alumno INT NOT NULL,
    id_palabra INT NOT NULL,
    id_partida INT NOT NULL,
    cantidad_fallos INT DEFAULT 1,
    ultima_vez_fallada TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_alumno_error FOREIGN KEY (id_alumno) REFERENCES USUARIO(id_usuario),
    CONSTRAINT fk_palabra_error FOREIGN KEY (id_palabra) REFERENCES PALABRA(id_palabra),
    CONSTRAINT fk_partida_error FOREIGN KEY (id_partida) REFERENCES PARTIDA(id_partida)
);

-- ID 1: Primaria Baja (Meteoritos lentos, salen cada 4 segs, palabras cortas)
INSERT INTO GRADO_ACADEMICO (nombre, velocidad_spawn, frecuencia_spawn, longitud_max_palabra) 
VALUES ('Primaria Baja', 100, 4, 5);

-- ID 2: Primaria Alta (Un poco más rápidos, salen cada 3 segs, palabras medianas)
INSERT INTO GRADO_ACADEMICO (nombre, velocidad_spawn, frecuencia_spawn, longitud_max_palabra) 
VALUES ('Primaria Alta', 150, 3, 8);

-- ID 3: Secundaria (Rápidos, salen cada 2 segs, palabras largas)
INSERT INTO GRADO_ACADEMICO (nombre, velocidad_spawn, frecuencia_spawn, longitud_max_palabra) 
VALUES ('Secundaria', 200, 2, 12);

-- ID 4: Preparatoria (Muy rápidos, salen cada 1.5 segs, palabras muy largas)
INSERT INTO GRADO_ACADEMICO (nombre, velocidad_spawn, frecuencia_spawn, longitud_max_palabra) 
VALUES ('Preparatoria', 280, 1, 20);

INSERT INTO USUARIO (username, nombre_completo, password_hash, ruta_foto_perfil, rol, activo, id_grado) 
VALUES (
    'S24016723', 
    'Ezequiel Eduardo Morales Domínguez', 
    'uv2026', 
    'res://assets/Perfiles/admin.jpg', 
    'Admin', 
    TRUE, 
    NULL
);

INSERT INTO USUARIO (username, nombre_completo, password_hash, ruta_foto_perfil, rol, activo, id_grado)
VALUES (
    'S24016728', 
    'Justin Dareh Pérez Montiel', 
    'soygay', 
    'res://assets/Perfiles/admin.jpg', 
    'Alumno', 
    TRUE, 
    1
);

INSERT INTO USUARIO (username, nombre_completo, password_hash, ruta_foto_perfil, rol, activo, id_grado)
VALUES (
    'S24016704', 
    'Christoper Robles Ricardez', 
    '1234', 
    'res://assets/Perfiles/admin.jpg', 
    'Alumno', 
    TRUE, 
    2
);

INSERT INTO USUARIO (username, nombre_completo, password_hash, ruta_foto_perfil, rol, activo, id_grado)
VALUES (
    'S24016717', 
    'Eric Ivan Macario lópez', 
    '1234', 
    'res://assets/Perfiles/admin.jpg', 
    'Alumno', 
    TRUE, 
    3
);

INSERT INTO USUARIO (username, nombre_completo, password_hash, ruta_foto_perfil, rol, activo, id_grado)
VALUES (
    'S24016724', 
    'Ezequiel Eduardo Morales Domínguez', 
    '1234', 
    'res://assets/Perfiles/admin.jpg', 
    'Alumno', 
    TRUE, 
    4
);