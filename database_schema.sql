--
-- PostgreSQL database dump
--

\restrict Cb3ohSZ3nmnpRi4PxeHVIUfVLgx6qTV9bohzggfDZX3Ozs8rLerNOSDRniqvo7B

-- Dumped from database version 14.19 (Homebrew)
-- Dumped by pg_dump version 14.19 (Homebrew)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: fn_update_vehicle_on_rental_complete(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_update_vehicle_on_rental_complete() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- RENTAL tablosunda status alanı 'completed' değerine güncellendiğinde
    IF NEW.status = 'completed' AND OLD.status IS DISTINCT FROM 'completed' THEN
        UPDATE VEHICLE
        SET status = 'available',
            -- dropoff_branch_id NULL ise mevcut branch_id korunur (null güvenlik kontrolü)
            branch_id = COALESCE(NEW.dropoff_branch_id, branch_id)
        WHERE vehicle_id = NEW.vehicle_id;
    END IF;
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.fn_update_vehicle_on_rental_complete() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: branch; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.branch (
    branch_id integer NOT NULL,
    city character varying(100) NOT NULL,
    address character varying(255) NOT NULL,
    phone character varying(20)
);


ALTER TABLE public.branch OWNER TO postgres;

--
-- Name: branch_branch_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.branch_branch_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.branch_branch_id_seq OWNER TO postgres;

--
-- Name: branch_branch_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.branch_branch_id_seq OWNED BY public.branch.branch_id;


--
-- Name: customer; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.customer (
    customer_id integer NOT NULL,
    first_name character varying(100) NOT NULL,
    last_name character varying(100) NOT NULL,
    license_number character varying(50) NOT NULL,
    birth_date date,
    email character varying(150),
    phone character varying(20),
    created_at timestamp without time zone DEFAULT now()
);


ALTER TABLE public.customer OWNER TO postgres;

--
-- Name: customer_customer_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.customer_customer_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.customer_customer_id_seq OWNER TO postgres;

--
-- Name: customer_customer_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.customer_customer_id_seq OWNED BY public.customer.customer_id;


--
-- Name: damage_report; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.damage_report (
    damage_id integer NOT NULL,
    rental_id integer NOT NULL,
    description character varying(500) NOT NULL,
    repair_cost numeric(10,2),
    report_date date DEFAULT CURRENT_DATE
);


ALTER TABLE public.damage_report OWNER TO postgres;

--
-- Name: damage_report_damage_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.damage_report_damage_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.damage_report_damage_id_seq OWNER TO postgres;

--
-- Name: damage_report_damage_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.damage_report_damage_id_seq OWNED BY public.damage_report.damage_id;


--
-- Name: feature; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.feature (
    feature_id integer NOT NULL,
    name character varying(100) NOT NULL,
    description character varying(255)
);


ALTER TABLE public.feature OWNER TO postgres;

--
-- Name: feature_feature_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.feature_feature_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.feature_feature_id_seq OWNER TO postgres;

--
-- Name: feature_feature_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.feature_feature_id_seq OWNED BY public.feature.feature_id;


--
-- Name: payment; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.payment (
    payment_id integer NOT NULL,
    rental_id integer,
    amount numeric(10,2) NOT NULL,
    payment_date timestamp without time zone DEFAULT now(),
    method character varying(30),
    CONSTRAINT payment_method_check CHECK (((method)::text = ANY ((ARRAY['credit_card'::character varying, 'cash'::character varying])::text[])))
);


ALTER TABLE public.payment OWNER TO postgres;

--
-- Name: payment_payment_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.payment_payment_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.payment_payment_id_seq OWNER TO postgres;

--
-- Name: payment_payment_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.payment_payment_id_seq OWNED BY public.payment.payment_id;


--
-- Name: rental; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.rental (
    rental_id integer NOT NULL,
    customer_id integer,
    vehicle_id integer,
    pickup_branch_id integer NOT NULL,
    dropoff_branch_id integer,
    start_date timestamp without time zone NOT NULL,
    end_date timestamp without time zone,
    total_amount numeric(10,2),
    status character varying(20),
    CONSTRAINT rental_status_check CHECK (((status)::text = ANY ((ARRAY['pending'::character varying, 'active'::character varying, 'completed'::character varying, 'cancelled'::character varying])::text[])))
);


ALTER TABLE public.rental OWNER TO postgres;

--
-- Name: rental_rental_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.rental_rental_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.rental_rental_id_seq OWNER TO postgres;

--
-- Name: rental_rental_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.rental_rental_id_seq OWNED BY public.rental.rental_id;


--
-- Name: staff; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.staff (
    staff_id integer NOT NULL,
    branch_id integer NOT NULL,
    first_name character varying(100) NOT NULL,
    last_name character varying(100) NOT NULL,
    role character varying(50) NOT NULL
);


ALTER TABLE public.staff OWNER TO postgres;

--
-- Name: staff_staff_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.staff_staff_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.staff_staff_id_seq OWNER TO postgres;

--
-- Name: staff_staff_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.staff_staff_id_seq OWNED BY public.staff.staff_id;


--
-- Name: vehicle; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.vehicle (
    vehicle_id integer NOT NULL,
    category_id integer,
    branch_id integer,
    plate_number character varying(20) NOT NULL,
    brand character varying(50) NOT NULL,
    model character varying(50) NOT NULL,
    year integer NOT NULL,
    daily_price numeric(10,2) NOT NULL,
    status character varying(20),
    mileage integer DEFAULT 0,
    CONSTRAINT vehicle_status_check CHECK (((status)::text = ANY ((ARRAY['available'::character varying, 'rented'::character varying, 'maintenance'::character varying])::text[])))
);


ALTER TABLE public.vehicle OWNER TO postgres;

--
-- Name: vehicle_category; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.vehicle_category (
    category_id integer NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(255)
);


ALTER TABLE public.vehicle_category OWNER TO postgres;

--
-- Name: vehicle_category_category_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.vehicle_category_category_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.vehicle_category_category_id_seq OWNER TO postgres;

--
-- Name: vehicle_category_category_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.vehicle_category_category_id_seq OWNED BY public.vehicle_category.category_id;


--
-- Name: vehicle_feature; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.vehicle_feature (
    vehicle_id integer NOT NULL,
    feature_id integer NOT NULL
);


ALTER TABLE public.vehicle_feature OWNER TO postgres;

--
-- Name: vehicle_vehicle_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.vehicle_vehicle_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.vehicle_vehicle_id_seq OWNER TO postgres;

--
-- Name: vehicle_vehicle_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.vehicle_vehicle_id_seq OWNED BY public.vehicle.vehicle_id;


--
-- Name: branch branch_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.branch ALTER COLUMN branch_id SET DEFAULT nextval('public.branch_branch_id_seq'::regclass);


--
-- Name: customer customer_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer ALTER COLUMN customer_id SET DEFAULT nextval('public.customer_customer_id_seq'::regclass);


--
-- Name: damage_report damage_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.damage_report ALTER COLUMN damage_id SET DEFAULT nextval('public.damage_report_damage_id_seq'::regclass);


--
-- Name: feature feature_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.feature ALTER COLUMN feature_id SET DEFAULT nextval('public.feature_feature_id_seq'::regclass);


--
-- Name: payment payment_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.payment ALTER COLUMN payment_id SET DEFAULT nextval('public.payment_payment_id_seq'::regclass);


--
-- Name: rental rental_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rental ALTER COLUMN rental_id SET DEFAULT nextval('public.rental_rental_id_seq'::regclass);


--
-- Name: staff staff_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.staff ALTER COLUMN staff_id SET DEFAULT nextval('public.staff_staff_id_seq'::regclass);


--
-- Name: vehicle vehicle_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.vehicle ALTER COLUMN vehicle_id SET DEFAULT nextval('public.vehicle_vehicle_id_seq'::regclass);


--
-- Name: vehicle_category category_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.vehicle_category ALTER COLUMN category_id SET DEFAULT nextval('public.vehicle_category_category_id_seq'::regclass);


--
-- Name: branch branch_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.branch
    ADD CONSTRAINT branch_pkey PRIMARY KEY (branch_id);


--
-- Name: customer customer_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_email_key UNIQUE (email);


--
-- Name: customer customer_license_number_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_license_number_key UNIQUE (license_number);


--
-- Name: customer customer_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_pkey PRIMARY KEY (customer_id);


--
-- Name: damage_report damage_report_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.damage_report
    ADD CONSTRAINT damage_report_pkey PRIMARY KEY (damage_id);


--
-- Name: feature feature_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.feature
    ADD CONSTRAINT feature_pkey PRIMARY KEY (feature_id);


--
-- Name: payment payment_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.payment
    ADD CONSTRAINT payment_pkey PRIMARY KEY (payment_id);


--
-- Name: payment payment_rental_id_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.payment
    ADD CONSTRAINT payment_rental_id_key UNIQUE (rental_id);


--
-- Name: rental rental_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rental
    ADD CONSTRAINT rental_pkey PRIMARY KEY (rental_id);


--
-- Name: staff staff_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.staff
    ADD CONSTRAINT staff_pkey PRIMARY KEY (staff_id);


--
-- Name: vehicle_category vehicle_category_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.vehicle_category
    ADD CONSTRAINT vehicle_category_pkey PRIMARY KEY (category_id);


--
-- Name: vehicle_feature vehicle_feature_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.vehicle_feature
    ADD CONSTRAINT vehicle_feature_pkey PRIMARY KEY (vehicle_id, feature_id);


--
-- Name: vehicle vehicle_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.vehicle
    ADD CONSTRAINT vehicle_pkey PRIMARY KEY (vehicle_id);


--
-- Name: vehicle vehicle_plate_number_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.vehicle
    ADD CONSTRAINT vehicle_plate_number_key UNIQUE (plate_number);


--
-- Name: rental trg_rental_completed; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_rental_completed AFTER UPDATE OF status ON public.rental FOR EACH ROW WHEN ((((new.status)::text = 'completed'::text) AND ((old.status)::text IS DISTINCT FROM 'completed'::text))) EXECUTE FUNCTION public.fn_update_vehicle_on_rental_complete();


--
-- Name: damage_report fk_damage_rental; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.damage_report
    ADD CONSTRAINT fk_damage_rental FOREIGN KEY (rental_id) REFERENCES public.rental(rental_id);


--
-- Name: payment fk_payment_rental; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.payment
    ADD CONSTRAINT fk_payment_rental FOREIGN KEY (rental_id) REFERENCES public.rental(rental_id);


--
-- Name: rental fk_rental_customer; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rental
    ADD CONSTRAINT fk_rental_customer FOREIGN KEY (customer_id) REFERENCES public.customer(customer_id);


--
-- Name: rental fk_rental_dropoff; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rental
    ADD CONSTRAINT fk_rental_dropoff FOREIGN KEY (dropoff_branch_id) REFERENCES public.branch(branch_id);


--
-- Name: rental fk_rental_pickup; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rental
    ADD CONSTRAINT fk_rental_pickup FOREIGN KEY (pickup_branch_id) REFERENCES public.branch(branch_id);


--
-- Name: rental fk_rental_vehicle; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rental
    ADD CONSTRAINT fk_rental_vehicle FOREIGN KEY (vehicle_id) REFERENCES public.vehicle(vehicle_id);


--
-- Name: staff fk_staff_branch; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.staff
    ADD CONSTRAINT fk_staff_branch FOREIGN KEY (branch_id) REFERENCES public.branch(branch_id);


--
-- Name: vehicle fk_vehicle_branch; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.vehicle
    ADD CONSTRAINT fk_vehicle_branch FOREIGN KEY (branch_id) REFERENCES public.branch(branch_id);


--
-- Name: vehicle fk_vehicle_category; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.vehicle
    ADD CONSTRAINT fk_vehicle_category FOREIGN KEY (category_id) REFERENCES public.vehicle_category(category_id);


--
-- Name: vehicle_feature fk_vf_feature; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.vehicle_feature
    ADD CONSTRAINT fk_vf_feature FOREIGN KEY (feature_id) REFERENCES public.feature(feature_id);


--
-- Name: vehicle_feature fk_vf_vehicle; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.vehicle_feature
    ADD CONSTRAINT fk_vf_vehicle FOREIGN KEY (vehicle_id) REFERENCES public.vehicle(vehicle_id);


--
-- PostgreSQL database dump complete
--

\unrestrict Cb3ohSZ3nmnpRi4PxeHVIUfVLgx6qTV9bohzggfDZX3Ozs8rLerNOSDRniqvo7B

