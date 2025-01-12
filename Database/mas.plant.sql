-- Table: mas.plant
-- Generated: 2024-12-19 13:29:38

CREATE TABLE mas.plant (
    plant_cd character varying(10) DEFAULT  NOT NULL,
    is_active boolean DEFAULT ,
    description character varying(50) DEFAULT ,
    pin_length integer(32) DEFAULT ,
    pin_reset_days integer(32) DEFAULT ,
    created_date timestamp with time zone DEFAULT  NOT NULL,
    last_edit_date timestamp with time zone DEFAULT  NOT NULL,
    color character varying(255) DEFAULT ,
    timezone character varying(25) DEFAULT ,
    PRIMARY KEY (plant_cd)
);

-- Permissions
GRANT SELECT ON mas.plant TO app_datareaders;
GRANT DELETE ON mas.plant TO app_datawriters;
GRANT INSERT ON mas.plant TO app_datawriters;
GRANT SELECT ON mas.plant TO app_datawriters;
GRANT UPDATE ON mas.plant TO app_datawriters;
