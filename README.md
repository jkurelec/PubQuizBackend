# PubQuizBackend

## Scaffold
Scaffold-DbContext "Host=localhost;Port=5432;Username=backend;Password=Pasvord123;Database=pub_quiz" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Model/DbModel -ContextDir Model -Context PubQuizContext -f

## DB da se ima
-- public.country definition

-- Drop table

-- DROP TABLE public.country;

CREATE TABLE public.country (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	"name" varchar(100) NOT NULL,
	country_code varchar(2) NOT NULL,
	CONSTRAINT country_pkey PRIMARY KEY (id)
);


-- public."user" definition

-- Drop table

-- DROP TABLE public."user";

CREATE TABLE public."user" (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	username varchar(255) NOT NULL,
	firstname varchar(255) NOT NULL,
	lastname varchar(255) NOT NULL,
	email varchar(255) NOT NULL,
	"password" varchar(255) NOT NULL,
	rating int4 DEFAULT 1000 NOT NULL,
	"role" int4 NOT NULL,
	CONSTRAINT host_pkey PRIMARY KEY (id),
	CONSTRAINT user_role_check CHECK ((role = ANY (ARRAY[1, 2, 3])))
);


-- public.city definition

-- Drop table

-- DROP TABLE public.city;

CREATE TABLE public.city (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	country_id int4 NOT NULL,
	"name" varchar(100) NOT NULL,
	CONSTRAINT city_pkey PRIMARY KEY (id),
	CONSTRAINT city_country_id_fkey FOREIGN KEY (country_id) REFERENCES public.country(id) ON DELETE CASCADE
);


-- public.organization definition

-- Drop table

-- DROP TABLE public.organization;

CREATE TABLE public.organization (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	"name" varchar(100) NOT NULL,
	editions_hosted int4 DEFAULT 0 NOT NULL,
	owner_id int4 NOT NULL,
	CONSTRAINT organizer_pkey PRIMARY KEY (id),
	CONSTRAINT fk_owner_id FOREIGN KEY (owner_id) REFERENCES public."user"(id) ON DELETE CASCADE
);


-- public.postal_code definition

-- Drop table

-- DROP TABLE public.postal_code;

CREATE TABLE public.postal_code (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	code varchar(10) NOT NULL,
	city_id int4 NOT NULL,
	CONSTRAINT postal_code_pkey PRIMARY KEY (id),
	CONSTRAINT postal_code_city_id_fkey FOREIGN KEY (city_id) REFERENCES public.city(id) ON DELETE CASCADE
);


-- public.quiz definition

-- Drop table

-- DROP TABLE public.quiz;

CREATE TABLE public.quiz (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	"name" varchar(100) NOT NULL,
	organization_id int4 NOT NULL,
	rating int4 DEFAULT 1000 NOT NULL,
	editions_hosted int4 DEFAULT 0 NOT NULL,
	CONSTRAINT quiz_pkey PRIMARY KEY (id),
	CONSTRAINT quiz_owner_id_fkey FOREIGN KEY (organization_id) REFERENCES public.organization(id) ON DELETE CASCADE
);


-- public.quiz_category definition

-- Drop table

-- DROP TABLE public.quiz_category;

CREATE TABLE public.quiz_category (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	"name" varchar(100) NOT NULL,
	super_category_id int4 NULL,
	CONSTRAINT category_pkey PRIMARY KEY (id),
	CONSTRAINT category_super_category_fkey FOREIGN KEY (super_category_id) REFERENCES public.quiz_category(id) ON DELETE CASCADE
);


-- public.quiz_league definition

-- Drop table

-- DROP TABLE public.quiz_league;

CREATE TABLE public.quiz_league (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	"name" varchar(255) NOT NULL,
	quiz_id int4 NOT NULL,
	points text NOT NULL,
	CONSTRAINT quiz_league_pkey PRIMARY KEY (id),
	CONSTRAINT quiz_league_quiz_id_fkey FOREIGN KEY (quiz_id) REFERENCES public.quiz(id)
);


-- public.quiz_quiz_category definition

-- Drop table

-- DROP TABLE public.quiz_quiz_category;

CREATE TABLE public.quiz_quiz_category (
	quiz_id int4 NOT NULL,
	quiz_category_id int4 NOT NULL,
	CONSTRAINT quiz_quiz_category_pkey PRIMARY KEY (quiz_id, quiz_category_id),
	CONSTRAINT quiz_quiz_category_quiz_category_id_fkey FOREIGN KEY (quiz_category_id) REFERENCES public.quiz_category(id) ON DELETE CASCADE,
	CONSTRAINT quiz_quiz_category_quiz_id_fkey FOREIGN KEY (quiz_id) REFERENCES public.quiz(id) ON DELETE CASCADE
);


-- public.refresh_token definition

-- Drop table

-- DROP TABLE public.refresh_token;

CREATE TABLE public.refresh_token (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	user_id int4 NOT NULL,
	"token" text NOT NULL,
	expires_at timestamp NOT NULL,
	app int4 NOT NULL,
	CONSTRAINT refresh_token_pkey PRIMARY KEY (id),
	CONSTRAINT fk_user_refresh FOREIGN KEY (user_id) REFERENCES public."user"(id) ON DELETE CASCADE
);


-- public.team definition

-- Drop table

-- DROP TABLE public.team;

CREATE TABLE public.team (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	owner_id int4 NOT NULL,
	"name" varchar(255) NOT NULL,
	rating int4 DEFAULT 0 NOT NULL,
	category_id int4 NOT NULL,
	quiz_id int4 NOT NULL,
	CONSTRAINT attendee_team_pkey PRIMARY KEY (id),
	CONSTRAINT attendee_team_category_id_fkey FOREIGN KEY (category_id) REFERENCES public.quiz_category(id) ON DELETE CASCADE,
	CONSTRAINT attendee_team_owner_id_fkey FOREIGN KEY (owner_id) REFERENCES public."user"(id) ON DELETE CASCADE,
	CONSTRAINT attendee_team_quiz_id_fkey FOREIGN KEY (quiz_id) REFERENCES public.quiz(id) ON DELETE CASCADE
);


-- public.user_team definition

-- Drop table

-- DROP TABLE public.user_team;

CREATE TABLE public.user_team (
	user_id int4 NOT NULL,
	team_id int4 NOT NULL,
	edit_team bool DEFAULT false NOT NULL,
	register_team bool DEFAULT false NOT NULL,
	delete_team bool DEFAULT false NOT NULL,
	CONSTRAINT user_team_pkey PRIMARY KEY (user_id, team_id),
	CONSTRAINT user_team_team_id_fkey FOREIGN KEY (team_id) REFERENCES public.team(id) ON DELETE CASCADE,
	CONSTRAINT user_team_user_id_fkey FOREIGN KEY (user_id) REFERENCES public."user"(id) ON DELETE CASCADE
);


-- public.host_organization_quiz definition

-- Drop table

-- DROP TABLE public.host_organization_quiz;

CREATE TABLE public.host_organization_quiz (
	host_id int4 NOT NULL,
	organization_id int4 NOT NULL,
	quiz_id int4 NOT NULL,
	create_edition bool DEFAULT false NOT NULL,
	edit_edition bool DEFAULT false NOT NULL,
	delete_edition bool DEFAULT false NOT NULL,
	CONSTRAINT host_organizer_quiz_pkey PRIMARY KEY (host_id, organization_id, quiz_id),
	CONSTRAINT host_organizer_quiz_host_id_fkey FOREIGN KEY (host_id) REFERENCES public."user"(id) ON DELETE CASCADE,
	CONSTRAINT host_organizer_quiz_organizer_id_fkey FOREIGN KEY (organization_id) REFERENCES public.organization(id) ON DELETE CASCADE,
	CONSTRAINT host_organizer_quiz_quiz_id_fkey FOREIGN KEY (quiz_id) REFERENCES public.quiz(id) ON DELETE CASCADE
);


-- public.league_prize definition

-- Drop table

-- DROP TABLE public.league_prize;

CREATE TABLE public.league_prize (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	league_id int4 NOT NULL,
	"name" varchar(255) NOT NULL,
	"position" int4 NULL,
	CONSTRAINT league_prize_pkey PRIMARY KEY (id),
	CONSTRAINT league_prize_league_id_fkey FOREIGN KEY (league_id) REFERENCES public.quiz_league(id)
);


-- public."location" definition

-- Drop table

-- DROP TABLE public."location";

CREATE TABLE public."location" (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	"name" varchar(100) NOT NULL,
	city_id int4 NOT NULL,
	gmaps_link text NULL,
	postal_code_id int4 NOT NULL,
	address varchar(255) NOT NULL,
	lat float8 NOT NULL,
	lon float8 NOT NULL,
	CONSTRAINT location_pkey PRIMARY KEY (id),
	CONSTRAINT fk_postal_code FOREIGN KEY (postal_code_id) REFERENCES public.postal_code(id) ON DELETE CASCADE,
	CONSTRAINT location_city_id_fkey FOREIGN KEY (city_id) REFERENCES public.city(id) ON DELETE CASCADE
);


-- public.quiz_edition definition

-- Drop table

-- DROP TABLE public.quiz_edition;

CREATE TABLE public.quiz_edition (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	"name" varchar(255) NOT NULL,
	quiz_id int4 NOT NULL,
	host_id int4 NOT NULL,
	category_id int4 NOT NULL,
	location_id int4 NOT NULL,
	"time" timestamptz NOT NULL,
	rating int4 NOT NULL,
	total_points numeric(10, 2) NOT NULL,
	fee_type int4 NULL,
	fee int4 NULL,
	duration int4 NULL,
	max_team_size int4 NULL,
	description text NULL,
	registration_start timestamptz NOT NULL,
	registration_end timestamptz NOT NULL,
	league_id int4 NULL,
	CONSTRAINT quiz_edition_fee_type_check CHECK (((fee_type >= 1) AND (fee_type <= 3))),
	CONSTRAINT quiz_edition_pkey PRIMARY KEY (id),
	CONSTRAINT fk_quiz_edition_league FOREIGN KEY (league_id) REFERENCES public.quiz_league(id),
	CONSTRAINT quiz_edition_category_id_fkey FOREIGN KEY (category_id) REFERENCES public.quiz_category(id) ON DELETE CASCADE,
	CONSTRAINT quiz_edition_host_fkey FOREIGN KEY (host_id) REFERENCES public."user"(id) ON DELETE CASCADE,
	CONSTRAINT quiz_edition_location_id_fkey FOREIGN KEY (location_id) REFERENCES public."location"(id) ON DELETE CASCADE,
	CONSTRAINT quiz_edition_quiz_id_fkey FOREIGN KEY (quiz_id) REFERENCES public.quiz(id) ON DELETE CASCADE
);


-- public.quiz_edition_results definition

-- Drop table

-- DROP TABLE public.quiz_edition_results;

CREATE TABLE public.quiz_edition_results (
	id serial4 NOT NULL,
	team_id int4 NOT NULL,
	edition_id int4 NOT NULL,
	"rank" int4 NOT NULL,
	total_points numeric(10, 2) NOT NULL,
	CONSTRAINT quiz_edition_results_pkey PRIMARY KEY (id),
	CONSTRAINT unique_team_quiz_edition UNIQUE (team_id, edition_id),
	CONSTRAINT quiz_edition_results_edition_id_fkey FOREIGN KEY (edition_id) REFERENCES public.quiz_edition(id) ON DELETE CASCADE,
	CONSTRAINT quiz_edition_results_team_id_fkey FOREIGN KEY (team_id) REFERENCES public.team(id) ON DELETE CASCADE
);


-- public.quiz_location definition

-- Drop table

-- DROP TABLE public.quiz_location;

CREATE TABLE public.quiz_location (
	quiz_id int4 NOT NULL,
	location_id int4 NOT NULL,
	CONSTRAINT quiz_location_pkey PRIMARY KEY (quiz_id, location_id),
	CONSTRAINT quiz_location_location_id_fkey FOREIGN KEY (location_id) REFERENCES public."location"(id) ON DELETE CASCADE,
	CONSTRAINT quiz_location_quiz_id_fkey FOREIGN KEY (quiz_id) REFERENCES public.quiz(id) ON DELETE CASCADE
);


-- public.quiz_round definition

-- Drop table

-- DROP TABLE public.quiz_round;

CREATE TABLE public.quiz_round (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	"number" int4 NOT NULL,
	edition_id int4 NOT NULL,
	CONSTRAINT round_pkey PRIMARY KEY (id),
	CONSTRAINT round_edition_id_fkey FOREIGN KEY (edition_id) REFERENCES public.quiz_edition(id) ON DELETE CASCADE
);


-- public.quiz_segment definition

-- Drop table

-- DROP TABLE public.quiz_segment;

CREATE TABLE public.quiz_segment (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	round_id int4 NOT NULL,
	bonus_points numeric(10, 2) DEFAULT 0 NULL,
	CONSTRAINT segment_pkey PRIMARY KEY (id),
	CONSTRAINT segment_round_id_fkey FOREIGN KEY (round_id) REFERENCES public.quiz_round(id) ON DELETE CASCADE
);


-- public.edition_prize definition

-- Drop table

-- DROP TABLE public.edition_prize;

CREATE TABLE public.edition_prize (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	edition_id int4 NOT NULL,
	"name" varchar(255) NOT NULL,
	"position" int4 NULL,
	CONSTRAINT prize_pkey PRIMARY KEY (id),
	CONSTRAINT prize_edition_id_fkey FOREIGN KEY (edition_id) REFERENCES public.quiz_edition(id)
);


-- public.quiz_question definition

-- Drop table

-- DROP TABLE public.quiz_question;

CREATE TABLE public.quiz_question (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	segment_id int4 NOT NULL,
	"type" int4 NOT NULL,
	question text NOT NULL,
	answer varchar(512) NOT NULL,
	points numeric(10, 2) DEFAULT 0 NOT NULL,
	bonus_points numeric(10, 2) DEFAULT 0 NULL,
	media_url varchar NULL,
	CONSTRAINT quiz_question_pkey PRIMARY KEY (id),
	CONSTRAINT quiz_question_segment_id_fkey FOREIGN KEY (segment_id) REFERENCES public.quiz_segment(id) ON DELETE CASCADE
);


-- public.quiz_answer definition

-- Drop table

-- DROP TABLE public.quiz_answer;

CREATE TABLE public.quiz_answer (
	id serial4 NOT NULL,
	team_id int4 NOT NULL,
	answer varchar(512) NULL,
	points numeric(10, 2) NOT NULL,
	question_id int4 NOT NULL,
	CONSTRAINT quiz_answer_pkey PRIMARY KEY (id),
	CONSTRAINT fk_quiz_answer_question FOREIGN KEY (question_id) REFERENCES public.quiz_question(id) ON DELETE CASCADE,
	CONSTRAINT quiz_answer_team_id_fkey FOREIGN KEY (team_id) REFERENCES public.team(id) ON DELETE CASCADE
);