# Data Base : PostgresQL : 

sudo apt update
sudo apt upgrade

sudo apt install postgresql postgresql-contrib

sudo systemctl status postgresql

sudo systemctl start postgresql

sudo -i -u postgres

psql

CREATE DATABASE taskmanagerdb;


CREATE USER taskuser WITH PASSWORD 'P@ssw0rd';


GRANT ALL PRIVILEGES ON DATABASE taskmanagerdb TO taskuser;

Verefication : 
psql -U taskuser -d taskmanagerdb -h 127.0.0.1 -W

# enter Password : 'P@ssw0rd'

************************************************************************
# Start  creation tables : 

GRANT ALL PRIVILEGES ON SCHEMA public TO taskuser;

# create table user : 

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL
);




#create table tasks

CREATE TABLE tasks (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES users(id) ON DELETE CASCADE,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);



# verification : 
\dt

************************************************************************

# start insert 

# new users 
INSERT INTO users (username, email, password_hash)
VALUES ('user1', 'user1@example.com', 'hashed_password1'),
       ('user2', 'user2@example.com', 'hashed_password2');

# new tasks 
INSERT INTO tasks (user_id, title, description, status)
VALUES (1, 'Task 1', 'First task description', 'Pending'),
       (1, 'Task 2', 'Second task description', 'Completed'),
       (2, 'Task 3', 'Third task description', 'Pending');


**********************************
# Conf 
# listen from all interfaces (not recommended)
listen_addresses = '*' 


#let access from windows machine : sudo nano /etc/postgresql/14/main/pg_hba.conf
host    all             all             0.0.0.0/0               md5


#if firewall 
sudo ufw allow 5432/tcp



--------------------------------------------------------------------------------------------
