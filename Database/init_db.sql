CREATE TABLE USUARIO (
    id_usuario SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    rol VARCHAR(20) NOT NULL,
    activo BOOLEAN DEFAULT TRUE
);

INSERT INTO USUARIO (username, password_hash, rol) VALUES ('eduardo', 'uv2026', 'Admin');
INSERT INTO USUARIO (username, password_hash, rol) VALUES ('alumno', '1234', 'Alumno');
INSERT INTO USUARIO (username, password_hash, rol) VALUES ('justin', '1234', 'Alumno');