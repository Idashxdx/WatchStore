--
-- PostgreSQL database dump
--

-- Dumped from database version 17.0 (Debian 17.0-1.pgdg120+1)
-- Dumped by pg_dump version 17.2 (Ubuntu 17.2-1.pgdg22.04+1)

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
-- Name: public; Type: SCHEMA; Schema: -; Owner: aidam
--

-- *not* creating schema, since initdb creates it


ALTER SCHEMA public OWNER TO aidam;

--
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: aidam
--

COMMENT ON SCHEMA public IS '';


--
-- Name: add_watch(text, text, integer, numeric, integer, text, text, numeric, text); Type: FUNCTION; Schema: public; Owner: aidam
--

CREATE FUNCTION public.add_watch(watch_name text, brand_name text, watch_type_id integer, watch_price numeric, watch_quantity integer, case_material text, strap_material text, case_diameter numeric, water_resistance text) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN
    INSERT INTO watches (name, brand, type_id, price, quantity, case_material, strap_material, case_diameter, water_resistance)
    VALUES (watch_name, brand_name, watch_type_id, watch_price, watch_quantity, case_material, strap_material, case_diameter, water_resistance);
END;
$$;


ALTER FUNCTION public.add_watch(watch_name text, brand_name text, watch_type_id integer, watch_price numeric, watch_quantity integer, case_material text, strap_material text, case_diameter numeric, water_resistance text) OWNER TO aidam;

--
-- Name: archive_closed_requests(); Type: FUNCTION; Schema: public; Owner: aidam
--

CREATE FUNCTION public.archive_closed_requests() RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Переносим в архив заявки, которые имеют статус "Closed"
    INSERT INTO archived_requests (id, user_id, description, employee_id, status_id, request_type, creation_date, archived_date)
    SELECT 
        id, user_id, description, employee_id, status_id, request_type, creation_date, NOW()
    FROM 
        watch_requests
    WHERE 
        status_id = (SELECT id FROM request_statuses WHERE name = 'Closed');

    -- Удаляем из таблицы активных заявок
    DELETE FROM watch_requests
    WHERE 
        status_id = (SELECT id FROM request_statuses WHERE name = 'Closed');
END;
$$;


ALTER FUNCTION public.archive_closed_requests() OWNER TO aidam;

--
-- Name: archive_order(integer, integer); Type: FUNCTION; Schema: public; Owner: aidam
--

CREATE FUNCTION public.archive_order(order_id integer, employee_id integer) RETURNS void
    LANGUAGE plpgsql
    AS $$
DECLARE
    order_to_archive RECORD;
BEGIN
    SELECT * INTO order_to_archive 
    FROM orders 
    WHERE id = order_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Заказ с ID % не найден.', order_id;
    END IF;

    INSERT INTO archived_orders (
        id,
        user_id,
        watch_id,
        status_id,
        quantity,
        order_date,
        delivery_address,
        archived_date
    )
    VALUES (
        order_to_archive.id,
        order_to_archive.user_id,
        order_to_archive.watch_id,
        order_to_archive.status_id,
        order_to_archive.quantity,
        order_to_archive.order_date,
        order_to_archive.delivery_address,
        NOW()
    );

    DELETE FROM orders WHERE id = order_id;
END;
$$;


ALTER FUNCTION public.archive_order(order_id integer, employee_id integer) OWNER TO aidam;

--
-- Name: delete_watch(integer); Type: FUNCTION; Schema: public; Owner: aidam
--

CREATE FUNCTION public.delete_watch(watch_id integer) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN
    DELETE FROM watches WHERE id = watch_id;
END;
$$;


ALTER FUNCTION public.delete_watch(watch_id integer) OWNER TO aidam;

--
-- Name: update_order_status(integer, integer, integer); Type: FUNCTION; Schema: public; Owner: aidam
--

CREATE FUNCTION public.update_order_status(order_id integer, new_status integer, employee_id integer) RETURNS void
    LANGUAGE plpgsql
    AS $$
DECLARE
    client_user_id INT;
BEGIN
    SELECT user_id INTO client_user_id FROM orders WHERE id = order_id;
    IF client_user_id IS NULL THEN
        RAISE EXCEPTION 'Не удалось найти клиента для заказа с ID %', order_id;
    END IF;

    UPDATE orders
    SET status_id = new_status
    WHERE id = order_id;

    IF new_status IN (4, 5) THEN
        PERFORM archive_order(order_id, employee_id);
    END IF;
END;
$$;


ALTER FUNCTION public.update_order_status(order_id integer, new_status integer, employee_id integer) OWNER TO aidam;

--
-- Name: update_request_status(integer, integer, integer); Type: FUNCTION; Schema: public; Owner: aidam
--

CREATE FUNCTION public.update_request_status(p_request_id integer, p_new_status_id integer, p_employee_id integer) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Если новый статус заявки — "Closed", переносим заявку в архив
    IF p_new_status_id = (SELECT id FROM request_statuses WHERE name = 'Closed') THEN
        -- Перенос в архив 
        INSERT INTO archived_requests (id, user_id, description, employee_id, status_id, request_type, creation_date, archived_date)
        SELECT id, user_id, description, employee_id, status_id, request_type, creation_date, NOW()
        FROM watch_requests
        WHERE id = p_request_id;

        -- Удаляем из активных заявок
        DELETE FROM watch_requests WHERE id = p_request_id;
    ELSE
        -- Обновляем статус и сотрудника для заявки
        UPDATE watch_requests
        SET status_id = p_new_status_id,
            employee_id = p_employee_id,
            creation_date = CURRENT_TIMESTAMP
        WHERE id = p_request_id;
    END IF;
END;
$$;


ALTER FUNCTION public.update_request_status(p_request_id integer, p_new_status_id integer, p_employee_id integer) OWNER TO aidam;

--
-- Name: update_watch_quantity_on_order(); Type: FUNCTION; Schema: public; Owner: aidam
--

CREATE FUNCTION public.update_watch_quantity_on_order() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
	UPDATE watches
	SET quantity = quantity - NEW.quantity
	WHERE id = NEW.watch_id;
	RETURN NEW;
END;
$$;


ALTER FUNCTION public.update_watch_quantity_on_order() OWNER TO aidam;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: aidam
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO aidam;

--
-- Name: archived_orders; Type: TABLE; Schema: public; Owner: aidam
--

CREATE TABLE public.archived_orders (
    id integer NOT NULL,
    user_id integer NOT NULL,
    watch_id integer NOT NULL,
    status_id integer NOT NULL,
    quantity integer NOT NULL,
    order_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    delivery_address text NOT NULL,
    archived_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT archived_orders_quantity_check CHECK ((quantity > 0))
);


ALTER TABLE public.archived_orders OWNER TO aidam;

--
-- Name: archived_orders_id_seq; Type: SEQUENCE; Schema: public; Owner: aidam
--

CREATE SEQUENCE public.archived_orders_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.archived_orders_id_seq OWNER TO aidam;

--
-- Name: archived_orders_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: aidam
--

ALTER SEQUENCE public.archived_orders_id_seq OWNED BY public.archived_orders.id;


--
-- Name: order_statuses; Type: TABLE; Schema: public; Owner: aidam
--

CREATE TABLE public.order_statuses (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(255)
);


ALTER TABLE public.order_statuses OWNER TO aidam;

--
-- Name: users; Type: TABLE; Schema: public; Owner: aidam
--

CREATE TABLE public.users (
    id integer NOT NULL,
    name character varying(255) NOT NULL,
    email character varying(255) NOT NULL,
    password_hash character varying(255) NOT NULL,
    phone character varying(15) NOT NULL,
    role_id integer NOT NULL,
    CONSTRAINT check_phone CHECK (((phone)::text ~ '^[0-9]{10,15}$'::text))
);


ALTER TABLE public.users OWNER TO aidam;

--
-- Name: watches; Type: TABLE; Schema: public; Owner: aidam
--

CREATE TABLE public.watches (
    id integer NOT NULL,
    name character varying(255) NOT NULL,
    brand character varying(255) NOT NULL,
    price numeric(10,2) NOT NULL,
    quantity integer DEFAULT 0,
    image_url character varying(255),
    type_id integer NOT NULL,
    case_material character varying(50),
    strap_material character varying(50),
    case_diameter numeric(4,1),
    water_resistance character varying(50),
    image bytea,
    CONSTRAINT watches_case_diameter_check CHECK ((case_diameter > (0)::numeric)),
    CONSTRAINT watches_price_check CHECK ((price > (0)::numeric)),
    CONSTRAINT watches_quantity_check CHECK ((quantity >= 0))
);


ALTER TABLE public.watches OWNER TO aidam;

--
-- Name: archived_orders_view; Type: VIEW; Schema: public; Owner: aidam
--

CREATE VIEW public.archived_orders_view AS
 SELECT ao.id AS order_id,
    u.id AS client_id,
    u.name AS client_name,
    w.name AS watch_name,
    w.brand,
    ao.quantity,
    os.name AS status,
    ao.order_date,
    ao.archived_date,
    ao.delivery_address
   FROM (((public.archived_orders ao
     LEFT JOIN public.users u ON ((ao.user_id = u.id)))
     LEFT JOIN public.watches w ON ((ao.watch_id = w.id)))
     LEFT JOIN public.order_statuses os ON ((ao.status_id = os.id)));


ALTER VIEW public.archived_orders_view OWNER TO aidam;

--
-- Name: archived_requests; Type: TABLE; Schema: public; Owner: aidam
--

CREATE TABLE public.archived_requests (
    id integer NOT NULL,
    user_id integer NOT NULL,
    description character varying(255) NOT NULL,
    employee_id integer,
    status_id integer NOT NULL,
    request_type character varying(50) NOT NULL,
    creation_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    archived_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.archived_requests OWNER TO aidam;

--
-- Name: archived_requests_id_seq; Type: SEQUENCE; Schema: public; Owner: aidam
--

CREATE SEQUENCE public.archived_requests_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.archived_requests_id_seq OWNER TO aidam;

--
-- Name: archived_requests_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: aidam
--

ALTER SEQUENCE public.archived_requests_id_seq OWNED BY public.archived_requests.id;


--
-- Name: employees; Type: TABLE; Schema: public; Owner: aidam
--

CREATE TABLE public.employees (
    id integer NOT NULL,
    user_id integer NOT NULL,
    "position" character varying(50) NOT NULL,
    hire_date date DEFAULT CURRENT_DATE
);


ALTER TABLE public.employees OWNER TO aidam;

--
-- Name: request_statuses; Type: TABLE; Schema: public; Owner: aidam
--

CREATE TABLE public.request_statuses (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(255)
);


ALTER TABLE public.request_statuses OWNER TO aidam;

--
-- Name: archived_requests_view; Type: VIEW; Schema: public; Owner: aidam
--

CREATE VIEW public.archived_requests_view AS
 SELECT ar.id AS request_id,
    u.id AS client_id,
    u.name AS client_name,
    ar.description,
    e."position" AS assigned_employee,
    rs.name AS status,
    ar.request_type,
    ar.creation_date,
    ar.archived_date
   FROM (((public.archived_requests ar
     LEFT JOIN public.users u ON ((ar.user_id = u.id)))
     LEFT JOIN public.employees e ON ((ar.employee_id = e.id)))
     LEFT JOIN public.request_statuses rs ON ((ar.status_id = rs.id)));


ALTER VIEW public.archived_requests_view OWNER TO aidam;

--
-- Name: watch_types; Type: TABLE; Schema: public; Owner: aidam
--

CREATE TABLE public.watch_types (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(255)
);


ALTER TABLE public.watch_types OWNER TO aidam;

--
-- Name: available_watches; Type: VIEW; Schema: public; Owner: aidam
--

CREATE VIEW public.available_watches AS
 SELECT w.id AS watch_id,
    w.name AS model_name,
    w.brand,
    wt.name AS type,
    w.price,
    w.quantity,
    w.case_material,
    w.strap_material,
    w.case_diameter,
    w.water_resistance,
    w.image
   FROM (public.watches w
     JOIN public.watch_types wt ON ((w.type_id = wt.id)))
  WHERE (w.quantity > 0);


ALTER VIEW public.available_watches OWNER TO aidam;

--
-- Name: roles; Type: TABLE; Schema: public; Owner: aidam
--

CREATE TABLE public.roles (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(255)
);


ALTER TABLE public.roles OWNER TO aidam;

--
-- Name: client_users; Type: VIEW; Schema: public; Owner: aidam
--

CREATE VIEW public.client_users AS
 SELECT u.id AS client_id,
    u.name AS client_name,
    u.email AS client_email,
    u.phone AS client_phone,
    r.name AS role_name
   FROM (public.users u
     JOIN public.roles r ON ((u.role_id = r.id)))
  WHERE ((r.name)::text = 'Client'::text);


ALTER VIEW public.client_users OWNER TO aidam;

--
-- Name: employees_id_seq; Type: SEQUENCE; Schema: public; Owner: aidam
--

CREATE SEQUENCE public.employees_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.employees_id_seq OWNER TO aidam;

--
-- Name: employees_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: aidam
--

ALTER SEQUENCE public.employees_id_seq OWNED BY public.employees.id;


--
-- Name: orders; Type: TABLE; Schema: public; Owner: aidam
--

CREATE TABLE public.orders (
    id integer NOT NULL,
    user_id integer NOT NULL,
    watch_id integer NOT NULL,
    status_id integer NOT NULL,
    quantity integer NOT NULL,
    order_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    delivery_address text NOT NULL,
    CONSTRAINT orders_quantity_check CHECK ((quantity > 0))
);


ALTER TABLE public.orders OWNER TO aidam;

--
-- Name: full_orders_history; Type: VIEW; Schema: public; Owner: aidam
--

CREATE VIEW public.full_orders_history AS
 SELECT o.id AS order_id,
    o.user_id,
    u.name AS client_name,
    w.name AS model_name,
    w.brand,
    o.quantity,
    os.name AS status,
    o.order_date,
    o.delivery_address,
    NULL::timestamp without time zone AS archived_date
   FROM (((public.orders o
     JOIN public.users u ON ((o.user_id = u.id)))
     JOIN public.watches w ON ((o.watch_id = w.id)))
     JOIN public.order_statuses os ON ((o.status_id = os.id)))
UNION ALL
 SELECT ao.id AS order_id,
    ao.user_id,
    u.name AS client_name,
    w.name AS model_name,
    w.brand,
    ao.quantity,
    os.name AS status,
    ao.order_date,
    ao.delivery_address,
    ao.archived_date
   FROM (((public.archived_orders ao
     JOIN public.users u ON ((ao.user_id = u.id)))
     JOIN public.watches w ON ((ao.watch_id = w.id)))
     JOIN public.order_statuses os ON ((ao.status_id = os.id)));


ALTER VIEW public.full_orders_history OWNER TO aidam;

--
-- Name: watch_requests; Type: TABLE; Schema: public; Owner: aidam
--

CREATE TABLE public.watch_requests (
    id integer NOT NULL,
    user_id integer NOT NULL,
    description character varying(255) NOT NULL,
    employee_id integer,
    status_id integer NOT NULL,
    request_type character varying(50) NOT NULL,
    creation_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    target_watch_name character varying(255),
    target_brand character varying(255),
    target_price_range numeric(10,2) DEFAULT NULL::numeric
);


ALTER TABLE public.watch_requests OWNER TO aidam;

--
-- Name: full_requests_history; Type: VIEW; Schema: public; Owner: aidam
--

CREATE VIEW public.full_requests_history AS
 SELECT wr.id AS request_id,
    wr.user_id,
    u.name AS client_name,
    wr.description,
    wr.request_type,
    rs.name AS status,
    wr.creation_date,
    NULL::timestamp without time zone AS archived_date
   FROM ((public.watch_requests wr
     JOIN public.users u ON ((u.id = wr.user_id)))
     JOIN public.request_statuses rs ON ((rs.id = wr.status_id)))
UNION ALL
 SELECT ar.id AS request_id,
    ar.user_id,
    u.name AS client_name,
    ar.description,
    ar.request_type,
    rs.name AS status,
    ar.creation_date,
    ar.archived_date
   FROM ((public.archived_requests ar
     JOIN public.users u ON ((u.id = ar.user_id)))
     JOIN public.request_statuses rs ON ((rs.id = ar.status_id)));


ALTER VIEW public.full_requests_history OWNER TO aidam;

--
-- Name: order_statuses_id_seq; Type: SEQUENCE; Schema: public; Owner: aidam
--

CREATE SEQUENCE public.order_statuses_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.order_statuses_id_seq OWNER TO aidam;

--
-- Name: order_statuses_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: aidam
--

ALTER SEQUENCE public.order_statuses_id_seq OWNED BY public.order_statuses.id;


--
-- Name: orders_id_seq; Type: SEQUENCE; Schema: public; Owner: aidam
--

CREATE SEQUENCE public.orders_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.orders_id_seq OWNER TO aidam;

--
-- Name: orders_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: aidam
--

ALTER SEQUENCE public.orders_id_seq OWNED BY public.orders.id;


--
-- Name: request_statuses_id_seq; Type: SEQUENCE; Schema: public; Owner: aidam
--

CREATE SEQUENCE public.request_statuses_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.request_statuses_id_seq OWNER TO aidam;

--
-- Name: request_statuses_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: aidam
--

ALTER SEQUENCE public.request_statuses_id_seq OWNED BY public.request_statuses.id;


--
-- Name: request_watches; Type: TABLE; Schema: public; Owner: aidam
--

CREATE TABLE public.request_watches (
    id integer NOT NULL,
    request_id integer NOT NULL,
    watch_id integer
);


ALTER TABLE public.request_watches OWNER TO aidam;

--
-- Name: request_watches_id_seq; Type: SEQUENCE; Schema: public; Owner: aidam
--

CREATE SEQUENCE public.request_watches_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.request_watches_id_seq OWNER TO aidam;

--
-- Name: request_watches_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: aidam
--

ALTER SEQUENCE public.request_watches_id_seq OWNED BY public.request_watches.id;


--
-- Name: roles_id_seq; Type: SEQUENCE; Schema: public; Owner: aidam
--

CREATE SEQUENCE public.roles_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.roles_id_seq OWNER TO aidam;

--
-- Name: roles_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: aidam
--

ALTER SEQUENCE public.roles_id_seq OWNED BY public.roles.id;


--
-- Name: user_orders; Type: VIEW; Schema: public; Owner: aidam
--

CREATE VIEW public.user_orders AS
 SELECT o.id AS order_id,
    u.id AS user_id,
    u.name AS client_name,
    w.name AS model_name,
    w.brand,
    o.quantity,
    os.name AS status,
    o.order_date,
    o.delivery_address
   FROM (((public.orders o
     JOIN public.users u ON ((o.user_id = u.id)))
     JOIN public.watches w ON ((o.watch_id = w.id)))
     JOIN public.order_statuses os ON ((o.status_id = os.id)));


ALTER VIEW public.user_orders OWNER TO aidam;

--
-- Name: user_requests; Type: VIEW; Schema: public; Owner: aidam
--

CREATE VIEW public.user_requests AS
 SELECT wr.id AS request_id,
    u.id AS user_id,
    u.name AS client_name,
    wr.description,
    wr.request_type,
    rs.name AS status,
    wr.creation_date,
    wr.target_watch_name,
    wr.target_brand,
    wr.target_price_range
   FROM ((public.watch_requests wr
     JOIN public.users u ON ((u.id = wr.user_id)))
     JOIN public.request_statuses rs ON ((rs.id = wr.status_id)));


ALTER VIEW public.user_requests OWNER TO aidam;

--
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: aidam
--

CREATE SEQUENCE public.users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_id_seq OWNER TO aidam;

--
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: aidam
--

ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;


--
-- Name: watch_types_id_seq; Type: SEQUENCE; Schema: public; Owner: aidam
--

CREATE SEQUENCE public.watch_types_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.watch_types_id_seq OWNER TO aidam;

--
-- Name: watch_types_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: aidam
--

ALTER SEQUENCE public.watch_types_id_seq OWNED BY public.watch_types.id;


--
-- Name: watches_id_seq; Type: SEQUENCE; Schema: public; Owner: aidam
--

CREATE SEQUENCE public.watches_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.watches_id_seq OWNER TO aidam;

--
-- Name: watches_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: aidam
--

ALTER SEQUENCE public.watches_id_seq OWNED BY public.watches.id;


--
-- Name: watches_requests_id_seq; Type: SEQUENCE; Schema: public; Owner: aidam
--

CREATE SEQUENCE public.watches_requests_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.watches_requests_id_seq OWNER TO aidam;

--
-- Name: watches_requests_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: aidam
--

ALTER SEQUENCE public.watches_requests_id_seq OWNED BY public.watch_requests.id;


--
-- Name: archived_orders id; Type: DEFAULT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.archived_orders ALTER COLUMN id SET DEFAULT nextval('public.archived_orders_id_seq'::regclass);


--
-- Name: archived_requests id; Type: DEFAULT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.archived_requests ALTER COLUMN id SET DEFAULT nextval('public.archived_requests_id_seq'::regclass);


--
-- Name: employees id; Type: DEFAULT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.employees ALTER COLUMN id SET DEFAULT nextval('public.employees_id_seq'::regclass);


--
-- Name: order_statuses id; Type: DEFAULT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.order_statuses ALTER COLUMN id SET DEFAULT nextval('public.order_statuses_id_seq'::regclass);


--
-- Name: orders id; Type: DEFAULT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.orders ALTER COLUMN id SET DEFAULT nextval('public.orders_id_seq'::regclass);


--
-- Name: request_statuses id; Type: DEFAULT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.request_statuses ALTER COLUMN id SET DEFAULT nextval('public.request_statuses_id_seq'::regclass);


--
-- Name: request_watches id; Type: DEFAULT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.request_watches ALTER COLUMN id SET DEFAULT nextval('public.request_watches_id_seq'::regclass);


--
-- Name: roles id; Type: DEFAULT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.roles ALTER COLUMN id SET DEFAULT nextval('public.roles_id_seq'::regclass);


--
-- Name: users id; Type: DEFAULT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);


--
-- Name: watch_requests id; Type: DEFAULT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.watch_requests ALTER COLUMN id SET DEFAULT nextval('public.watches_requests_id_seq'::regclass);


--
-- Name: watch_types id; Type: DEFAULT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.watch_types ALTER COLUMN id SET DEFAULT nextval('public.watch_types_id_seq'::regclass);


--
-- Name: watches id; Type: DEFAULT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.watches ALTER COLUMN id SET DEFAULT nextval('public.watches_id_seq'::regclass);


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: archived_orders archived_orders_pkey; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.archived_orders
    ADD CONSTRAINT archived_orders_pkey PRIMARY KEY (id);


--
-- Name: archived_requests archived_requests_pkey; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.archived_requests
    ADD CONSTRAINT archived_requests_pkey PRIMARY KEY (id);


--
-- Name: employees employees_pkey; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (id);


--
-- Name: employees employees_user_id_key; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_user_id_key UNIQUE (user_id);


--
-- Name: order_statuses order_statuses_name_key; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.order_statuses
    ADD CONSTRAINT order_statuses_name_key UNIQUE (name);


--
-- Name: order_statuses order_statuses_pkey; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.order_statuses
    ADD CONSTRAINT order_statuses_pkey PRIMARY KEY (id);


--
-- Name: orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (id);


--
-- Name: request_statuses request_statuses_name_key; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.request_statuses
    ADD CONSTRAINT request_statuses_name_key UNIQUE (name);


--
-- Name: request_statuses request_statuses_pkey; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.request_statuses
    ADD CONSTRAINT request_statuses_pkey PRIMARY KEY (id);


--
-- Name: request_watches request_watches_pkey; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.request_watches
    ADD CONSTRAINT request_watches_pkey PRIMARY KEY (id);


--
-- Name: roles roles_name_key; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_name_key UNIQUE (name);


--
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);


--
-- Name: users users_email_key; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- Name: users users_phone_key; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_phone_key UNIQUE (phone);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- Name: watch_types watch_types_name_key; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.watch_types
    ADD CONSTRAINT watch_types_name_key UNIQUE (name);


--
-- Name: watch_types watch_types_pkey; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.watch_types
    ADD CONSTRAINT watch_types_pkey PRIMARY KEY (id);


--
-- Name: watches watches_pkey; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.watches
    ADD CONSTRAINT watches_pkey PRIMARY KEY (id);


--
-- Name: watch_requests watches_requests_pkey; Type: CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.watch_requests
    ADD CONSTRAINT watches_requests_pkey PRIMARY KEY (id);


--
-- Name: orders reduce_watch_quantity; Type: TRIGGER; Schema: public; Owner: aidam
--

CREATE TRIGGER reduce_watch_quantity AFTER INSERT ON public.orders FOR EACH ROW EXECUTE FUNCTION public.update_watch_quantity_on_order();


--
-- Name: archived_orders archived_orders_status_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.archived_orders
    ADD CONSTRAINT archived_orders_status_id_fkey FOREIGN KEY (status_id) REFERENCES public.order_statuses(id);


--
-- Name: archived_orders archived_orders_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.archived_orders
    ADD CONSTRAINT archived_orders_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: archived_orders archived_orders_watch_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.archived_orders
    ADD CONSTRAINT archived_orders_watch_id_fkey FOREIGN KEY (watch_id) REFERENCES public.watches(id) ON DELETE CASCADE;


--
-- Name: archived_requests archived_requests_employee_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.archived_requests
    ADD CONSTRAINT archived_requests_employee_id_fkey FOREIGN KEY (employee_id) REFERENCES public.employees(id) ON DELETE SET NULL;


--
-- Name: archived_requests archived_requests_status_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.archived_requests
    ADD CONSTRAINT archived_requests_status_id_fkey FOREIGN KEY (status_id) REFERENCES public.request_statuses(id);


--
-- Name: archived_requests archived_requests_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.archived_requests
    ADD CONSTRAINT archived_requests_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: employees employees_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: orders orders_status_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_status_id_fkey FOREIGN KEY (status_id) REFERENCES public.order_statuses(id);


--
-- Name: orders orders_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: orders orders_watch_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_watch_id_fkey FOREIGN KEY (watch_id) REFERENCES public.watches(id) ON DELETE CASCADE;


--
-- Name: request_watches request_watches_request_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.request_watches
    ADD CONSTRAINT request_watches_request_id_fkey FOREIGN KEY (request_id) REFERENCES public.watch_requests(id) ON DELETE CASCADE;


--
-- Name: request_watches request_watches_watch_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.request_watches
    ADD CONSTRAINT request_watches_watch_id_fkey FOREIGN KEY (watch_id) REFERENCES public.watches(id) ON DELETE CASCADE;


--
-- Name: users users_role_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_role_id_fkey FOREIGN KEY (role_id) REFERENCES public.roles(id);


--
-- Name: watch_requests watches_requests_employee_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.watch_requests
    ADD CONSTRAINT watches_requests_employee_id_fkey FOREIGN KEY (employee_id) REFERENCES public.employees(id) ON DELETE SET NULL;


--
-- Name: watch_requests watches_requests_status_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.watch_requests
    ADD CONSTRAINT watches_requests_status_id_fkey FOREIGN KEY (status_id) REFERENCES public.request_statuses(id);


--
-- Name: watch_requests watches_requests_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.watch_requests
    ADD CONSTRAINT watches_requests_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(id) ON DELETE CASCADE;


--
-- Name: watches watches_type_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: aidam
--

ALTER TABLE ONLY public.watches
    ADD CONSTRAINT watches_type_id_fkey FOREIGN KEY (type_id) REFERENCES public.watch_types(id) ON DELETE SET NULL;


--
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: aidam
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- PostgreSQL database dump complete
--

