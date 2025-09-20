--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5 (Debian 17.5-1.pgdg120+1)
-- Dumped by pg_dump version 17.5 (Debian 17.5-1.pgdg120+1)

-- Started on 2025-07-26 06:48:08 UTC

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

DROP DATABASE IF EXISTS postgres;
--
-- TOC entry 3388 (class 1262 OID 5)
-- Name: postgres; Type: DATABASE; Schema: -; Owner: -
--

CREATE DATABASE postgres WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';


\connect postgres

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 3389 (class 0 OID 0)
-- Dependencies: 3388
-- Name: DATABASE postgres; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON DATABASE postgres IS 'default administrative connection database';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 218 (class 1259 OID 16467)
-- Name: compensationrequests; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.compensationrequests (
    id integer NOT NULL,
    employeeid bigint NOT NULL,
    workdate timestamp with time zone NOT NULL,
    requestedcompensationdate timestamp with time zone NOT NULL,
    status text NOT NULL,
    justification text,
    approvedbyid bigint,
    requestedat timestamp with time zone NOT NULL,
    decidedat timestamp with time zone
);


--
-- TOC entry 217 (class 1259 OID 16466)
-- Name: compensationrequests_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.compensationrequests_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 3390 (class 0 OID 0)
-- Dependencies: 217
-- Name: compensationrequests_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.compensationrequests_id_seq OWNED BY public.compensationrequests.id;


--
-- TOC entry 219 (class 1259 OID 16475)
-- Name: employees; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.employees (
    id bigint NOT NULL,
    name character varying(100) NOT NULL,
    "position" character varying(50),
    salary double precision,
    manager_id bigint
);


--
-- TOC entry 221 (class 1259 OID 16483)
-- Name: managers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.managers (
    manager_id bigint NOT NULL,
    manager_name text NOT NULL
);


--
-- TOC entry 220 (class 1259 OID 16478)
-- Name: users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.users (
    id bigint NOT NULL,
    email text NOT NULL,
    name text NOT NULL,
    password text NOT NULL,
    role text NOT NULL,
    username text NOT NULL
);


--
-- TOC entry 222 (class 1259 OID 16485)
-- Name: extra_hours; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.extra_hours (
    registry integer NOT NULL,
    id bigint NOT NULL,
    date timestamp with time zone NOT NULL,
    "startTime" interval NOT NULL,
    "endTime" interval NOT NULL,
    diurnal double precision NOT NULL,
    nocturnal double precision NOT NULL,
    "diurnalHoliday" double precision NOT NULL,
    "nocturnalHoliday" double precision NOT NULL,
    "extraHours" double precision NOT NULL,
    observations text,
    approved boolean NOT NULL,
    approved_by_manager_id bigint
);


--
-- TOC entry 223 (class 1259 OID 16487)
-- Name: extra_hours_config; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.extra_hours_config (
    id bigint NOT NULL,
    "weeklyExtraHoursLimit" double precision NOT NULL,
    "diurnalMultiplier" double precision NOT NULL,
    "nocturnalMultiplier" double precision NOT NULL,
    "diurnalHolidayMultiplier" double precision NOT NULL,
    "nocturnalHolidayMultiplier" double precision NOT NULL,
    "diurnalStart" time without time zone NOT NULL,
    "diurnalEnd" time without time zone NOT NULL
);


--
-- TOC entry 224 (class 1259 OID 16489)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


--
-- TOC entry 3222 (class 2604 OID 16470)
-- Name: compensationrequests id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.compensationrequests ALTER COLUMN id SET DEFAULT nextval('public.compensationrequests_id_seq'::regclass);


--
-- TOC entry 3379 (class 0 OID 16467)
-- Dependencies: 218
-- Data for Name: compensationrequests; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.compensationrequests (id, employeeid, workdate, requestedcompensationdate, status, justification, approvedbyid, requestedat, decidedat) FROM stdin;
\.


--
-- TOC entry 3380 (class 0 OID 16475)
-- Dependencies: 219
-- Data for Name: employees; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.employees (id, name, "position", salary, manager_id) FROM stdin;
123473	Diego López Ramírez	CallCenter nivel 1	5220620	1
123541	Isabel Pineda Salcedo	Técnico de operaciones	5472593	2
123495	Antonio Salcedo López	Técnico de operaciones	7929343	2
123525	Ana Paula Montoya Estrada	Analista nivel 1	6800936	2
123643	Lina Morales Díaz	Gerente de proyectos	3429870	4
123529	Julio Calderón Pineda	Consultor financiero	4030289	4
123462	Miguel Ángel Torres Jiménez	Gerente de proyectos	7891234	5
12349	Jose Miguel Ospina 	Analista nivel 3	6500000	2
123456	Ana María García López	Analista nivel 2	4500000	1
654321	Juan Peréz	Analista nivel 3	4500000	2
\.


--
-- TOC entry 3382 (class 0 OID 16483)
-- Dependencies: 221
-- Data for Name: managers; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.managers (manager_id, manager_name) FROM stdin;
1	Carlos Miguel Pérez Sánchez
2	Laura Patricia Gómez Torres
3	María Fernanda Sánchez Morales
4	Luis Javier Fernández Gutiérrez
5	Paula Andrea Ramírez Quintana
\.


--
-- TOC entry 3381 (class 0 OID 16478)
-- Dependencies: 220
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.users (id, email, name, password, role, username) FROM stdin;
3	maria.sanchez@example.com	María Fernanda Sánchez Morales	$2a$11$bfIDtLD1BZlpvmoyt25eH.oqIRE1pKaIuXL3VPTgUFVqGP6ENsLNK	manager	maria_sanchez
4	luis.fernandez@example.com	Luis Javier Fernández Gutiérrez	$2a$11$bfIDtLD1BZlpvmoyt25eH.oqIRE1pKaIuXL3VPTgUFVqGP6ENsLNK	manager	luis_fernandez
5	paula.ramirez@example.com	Paula Andrea Ramírez Quintana	$2a$11$bfIDtLD1BZlpvmoyt25eH.oqIRE1pKaIuXL3VPTgUFVqGP6ENsLNK	manager	paula_ramirez
123473	diego.lopez@example.com	Diego López Ramírez	$2a$11$bfIDtLD1BZlpvmoyt25eH.oqIRE1pKaIuXL3VPTgUFVqGP6ENsLNK	empleado	diego_lopez
123541	isabel.pineda@example.com	Isabel Pineda Salcedo	$2a$11$bfIDtLD1BZlpvmoyt25eH.oqIRE1pKaIuXL3VPTgUFVqGP6ENsLNK	empleado	isabel_pineda
123495	antonio.salcedo@example.com	Antonio Salcedo López	$2a$11$bfIDtLD1BZlpvmoyt25eH.oqIRE1pKaIuXL3VPTgUFVqGP6ENsLNK	empleado	antonio_salcedo
123525	anapaula.montoya@example.com	Ana Paula Montoya Estrada	$2a$11$bfIDtLD1BZlpvmoyt25eH.oqIRE1pKaIuXL3VPTgUFVqGP6ENsLNK	empleado	anapaula_montoya
123643	lina.morales@example.com	Lina Morales Díaz	$2a$11$bfIDtLD1BZlpvmoyt25eH.oqIRE1pKaIuXL3VPTgUFVqGP6ENsLNK	empleado	lina_morales
123529	julio.calderon@example.com	Julio Calderón Pineda	$2a$11$bfIDtLD1BZlpvmoyt25eH.oqIRE1pKaIuXL3VPTgUFVqGP6ENsLNK	empleado	julio_calderon
654321	juan.peréz@empresa.com	Juan Peréz	$2a$11$.GDnP3HMrFBlqVaae0icPemnm4ILWSGOGrL8.bNDRs9k93TyaVN2.	empleado	juan.peréz
123462	miguel.torres@example.com	Miguel Ángel Torres Jiménez	$2a$11$d90xNNYtc5Hj22bf5L/Js.y3O8XTNxjl/ThO0yIbp4dr.pCt5SaIW	empleado	miguel_torres
2	laura.gomez@example.com	Laura Patricia Gómez Torres	$2a$11$1QOaFBTiLbleZPim1cmdDucYuN4AOV/8IlDd14O21JqBq3M/k7iSK	superusuario	laura_gomez
1	jaialgallo@gmail.com	Carlos Miguel Pérez Sánchez	$2a$11$bfIDtLD1BZlpvmoyt25eH.oqIRE1pKaIuXL3VPTgUFVqGP6ENsLNK	manager	carlos_perez
12349	jose.miguel.ospina.@empresa.com	Jose Miguel Ospina 	$2a$11$3svdu7h1LR59PDZcif3wbeijWVd54Pp82RtLr.psKThPHZKt0s7TS	empleado	jose.miguel.ospina.
123456	jagallob@eafit.edu.co	Ana María García López	$2a$11$bfIDtLD1BZlpvmoyt25eH.oqIRE1pKaIuXL3VPTgUFVqGP6ENsLNK	empleado	ana_garcia
\.


--
-- TOC entry 3383 (class 0 OID 16485)
-- Dependencies: 222
-- Data for Name: extra_hours; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.extra_hours (registry, id, date, "startTime", "endTime", diurnal, nocturnal, "diurnalHoliday", "nocturnalHoliday", "extraHours", observations, approved, approved_by_manager_id) FROM stdin;
112	123456	2025-04-11 00:00:00-05	20:00:00	22:00:00	1	1	0	0	2	Corrección de errores	f	\N
135	123456	2025-04-04 00:00:00-05	13:06:00	14:24:00	1.3	0	0	0	1.3	Mantenimiento sistema horas extra	t	1
106	123456	2025-04-05 00:00:00-05	17:30:00	19:30:00	2	0	0	0	2	Desarrollo módulo reportes	t	1
114	123462	2025-04-06 00:00:00-05	07:04:00	10:04:00	0	0	3	0	3	Planificación trimestral	f	\N
115	123462	2025-04-07 00:00:00-05	21:05:00	23:05:00	0	2	0	0	2	Reunión con clientes	f	\N
116	123462	2025-04-08 00:00:00-05	19:05:00	21:30:00	1.92	0.5	0	0	2.42	Presentación proyecto	f	\N
119	123495	2025-04-05 00:00:00-05	18:08:00	21:08:00	2.87	0.13	0	0	3	Mantenimiento preventivo	f	\N
120	123541	2025-04-05 00:00:00-05	17:10:00	19:30:00	2.33	0	0	0	2.33	Mantenimiento equipos	f	\N
121	123541	2025-04-06 00:00:00-05	08:00:00	11:00:00	0	0	3	0	3	Actualización sistemas	f	\N
122	123541	2025-04-07 00:00:00-05	19:00:00	21:00:00	2	0	0	0	2	Revisión protocolos	f	\N
123	123541	2025-04-08 00:00:00-05	06:00:00	08:30:00	2.5	0	0	0	2.5	Reinstalación software	f	\N
124	123541	2025-04-09 00:00:00-05	18:00:00	21:00:00	3	0	0	0	3	Reparación emergencia	f	\N
125	123541	2025-04-10 00:00:00-05	19:30:00	21:30:00	1.5	0.5	0	0	2	Actualización firmware	f	\N
126	123541	2025-04-11 00:00:00-05	17:00:00	19:00:00	2	0	0	0	2	Capacitación equipos	f	\N
132	123456	2025-04-04 00:00:00-05	12:25:00	13:25:00	1	0	0	0	1	Mantenimiento sistema	f	\N
133	123456	2025-04-04 00:00:00-05	12:32:00	13:46:00	1.23	0	0	0	1.23	Mantenimiento sistema	f	\N
134	123456	2025-04-04 00:00:00-05	13:03:00	14:21:00	1.3	0	0	0	1.3	Mantenimiento sistema	f	\N
107	123456	2025-04-06 00:00:00-05	19:00:00	22:00:00	0	0	2	1	3	Migración de datos	t	1
117	123473	2025-04-05 00:00:00-05	19:07:00	21:07:00	1.88	0.12	0	0	2	Atención extendida clientes	t	1
128	123643	2025-04-06 00:00:00-05	09:00:00	12:00:00	0	0	3	0	3	Reunión clientes	t	2
109	123456	2025-04-08 00:00:00-05	22:00:00	23:30:00	0	1.5	0	0	1.5	Mantenimiento sistema	t	1
140	123456	2025-04-04 00:00:00-05	17:35:00	19:35:00	0.42	1.58	0	0	2	Actualizar registro	f	\N
111	123456	2025-04-10 00:00:00-05	06:00:00	08:00:00	2	0	0	0	2	Reunión internacional	t	1
136	123456	2025-04-04 00:00:00-05	14:48:00	15:48:00	1	0	0	0	1	Actualización registro	f	\N
130	123643	2025-04-08 00:00:00-05	17:00:00	19:30:00	2.5	0	0	0	2.5	Revisión cronograma	t	2
137	123456	2025-04-04 00:00:00-05	20:16:00	22:26:00	0.73	1.43	0	0	2.16	Actualizar registro hora extra	t	2
139	123456	2025-04-04 00:00:00-05	17:29:00	19:29:00	0.52	1.48	0	0	2	Actualizar registro	f	\N
141	123456	2025-04-04 00:00:00-05	17:40:00	19:40:00	0.33	1.67	0	0	2	Actualizar registro	f	\N
151	123456	2025-06-10 00:00:00-05	08:50:00	09:36:00	0.77	0	0	0	0.77	Propuesta	t	2
113	123462	2025-04-05 00:00:00-05	18:00:00	20:00:00	2	0	0	0	2	Revisión de presupuestos	t	2
142	123456	2025-05-01 00:00:00-05	19:46:00	21:00:00	0	0	0	1.23	1.23	Configurar Azure	f	\N
143	123456	2025-05-10 00:00:00-05	11:00:00	11:30:00	0.5	0	0	0	0.5	Corregir riesgo expuesto	f	\N
144	123456	2025-05-10 00:00:00-05	15:39:00	16:39:00	1	0	0	0	1	Refactorización servicios	f	\N
145	123456	2025-05-10 00:00:00-05	16:36:00	18:36:00	1.4	0.6	0	0	2	Resolver conflictos	f	\N
146	123456	2025-05-10 00:00:00-05	16:57:00	21:57:00	1.05	3.95	0	0	5	Prueba	f	\N
147	123462	2025-05-10 00:00:00-05	16:59:00	21:59:00	1.02	3.98	0	0	5	Ensayo	f	\N
148	123456	2025-05-23 00:00:00-05	15:59:00	16:59:00	1	0	0	0	1	Pruebas adicionales	f	\N
150	123456	2025-06-02 00:00:00-05	00:53:00	01:53:00	0	0	0	1	1	Prueba	f	\N
\.


--
-- TOC entry 3384 (class 0 OID 16487)
-- Dependencies: 223
-- Data for Name: extra_hours_config; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.extra_hours_config (id, "weeklyExtraHoursLimit", "diurnalMultiplier", "nocturnalMultiplier", "diurnalHolidayMultiplier", "nocturnalHolidayMultiplier", "diurnalStart", "diurnalEnd") FROM stdin;
1	12	0	0	0	0	06:00:00	21:00:00
\.


--
-- TOC entry 3385 (class 0 OID 16489)
-- Dependencies: 224
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20250223174723_InitialCreate	9.0.2
20250318031719_AddUserRelationsAndApprovedByManager	9.0.2
20250402042947_AddArticlesTable	9.0.2
20250328030128_InitialCreate	9.0.2
20250619183116_AddCompensationRequest	9.0.2
20250726173722_FixCompensationRequestTableName	9.0.2
\.


--
-- TOC entry 3391 (class 0 OID 0)
-- Dependencies: 217
-- Name: compensationrequests_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.compensationrequests_id_seq', 1, false);


--
-- TOC entry 3392 (class 0 OID 0)
-- Dependencies: 217
-- Name: extra_hours_registry_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.extra_hours_registry_seq', 151, true);


--
-- TOC entry 3393 (class 0 OID 0)
-- Dependencies: 217
-- Name: extra_hours_config_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.extra_hours_config_id_seq', 1, false);


--
-- TOC entry 3224 (class 2606 OID 16474)
-- Name: compensationrequests compensationrequests_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.compensationrequests
    ADD CONSTRAINT compensationrequests_pkey PRIMARY KEY (id);


--
-- TOC entry 3226 (class 2606 OID 16489)
-- Name: employees pk_employees; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT pk_employees PRIMARY KEY (id);


--
-- TOC entry 3230 (class 2606 OID 16493)
-- Name: managers pk_managers; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.managers
    ADD CONSTRAINT pk_managers PRIMARY KEY (manager_id);


--
-- TOC entry 3228 (class 2606 OID 16491)
-- Name: users pk_users; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT pk_users PRIMARY KEY (id);


--
-- TOC entry 3233 (class 2606 OID 16495)
-- Name: extra_hours PK_extra_hours; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.extra_hours
    ADD CONSTRAINT "PK_extra_hours" PRIMARY KEY (registry);


--
-- TOC entry 3234 (class 2606 OID 16497)
-- Name: extra_hours_config PK_extra_hours_config; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.extra_hours_config
    ADD CONSTRAINT "PK_extra_hours_config" PRIMARY KEY (id);


--
-- TOC entry 3235 (class 2606 OID 16499)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- TOC entry 3231 (class 2606 OID 16494)
-- Name: compensationrequests fk_compensationrequests_employees_employeeid; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.compensationrequests
    ADD CONSTRAINT fk_compensationrequests_employees_employeeid FOREIGN KEY (employeeid) REFERENCES public.employees(id) ON DELETE CASCADE;


--
-- TOC entry 3232 (class 2606 OID 16499)
-- Name: compensationrequests fk_compensationrequests_users_approvedbyid; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.compensationrequests
    ADD CONSTRAINT fk_compensationrequests_users_approvedbyid FOREIGN KEY (approvedbyid) REFERENCES public.users(id) ON DELETE SET NULL;


--
-- TOC entry 3236 (class 2606 OID 16501)
-- Name: employees FK_employees_managers_manager_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT "FK_employees_managers_manager_id" FOREIGN KEY (manager_id) REFERENCES public.managers(manager_id) ON DELETE SET NULL;


--
-- TOC entry 3237 (class 2606 OID 16503)
-- Name: employees FK_employees_users_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT "FK_employees_users_id" FOREIGN KEY (id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- TOC entry 3238 (class 2606 OID 16505)
-- Name: extra_hours FK_extra_hours_employees_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.extra_hours
    ADD CONSTRAINT "FK_extra_hours_employees_id" FOREIGN KEY (id) REFERENCES public.employees(id) ON DELETE CASCADE;


--
-- TOC entry 3239 (class 2606 OID 16507)
-- Name: extra_hours FK_extra_hours_managers_approved_by_manager_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.extra_hours
    ADD CONSTRAINT "FK_extra_hours_managers_approved_by_manager_id" FOREIGN KEY (approved_by_manager_id) REFERENCES public.managers(manager_id);


--
-- TOC entry 3240 (class 2606 OID 16509)
-- Name: managers FK_managers_users_manager_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.managers
    ADD CONSTRAINT "FK_managers_users_manager_id" FOREIGN KEY (manager_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- TOC entry 3241 (class 2606 OID 16511)
-- Name: IX_employees_manager_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_employees_manager_id" ON public.employees USING btree (manager_id);


--
-- TOC entry 3242 (class 2606 OID 16513)
-- Name: IX_extra_hours_approved_by_manager_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_extra_hours_approved_by_manager_id" ON public.extra_hours USING btree (approved_by_manager_id);


--
-- TOC entry 3243 (class 2606 OID 16515)
-- Name: IX_extra_hours_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_extra_hours_id" ON public.extra_hours USING btree (id);


--
-- TOC entry 3244 (class 2606 OID 16517)
-- Name: IX_compensationrequests_employeeid; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_compensationrequests_employeeid" ON public.compensationrequests USING btree (employeeid);


--
-- TOC entry 3245 (class 2606 OID 16519)
-- Name: IX_compensationrequests_approvedbyid; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_compensationrequests_approvedbyid" ON public.compensationrequests USING btree (approvedbyid);


-- Completed on 2025-07-26 06:48:11 UTC

--
-- PostgreSQL database dump complete
--

