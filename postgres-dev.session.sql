CREATE TABLE Barbecue (
  id uuid PRIMARY KEY,
  name text NOT NULL,
  description text NOT NULL,
  date TIMESTAMP NOT NULL
);

CREATE TABLE Participant (
  id serial PRIMARY KEY,
  name text NOT NULL,
  contribution INT NOT NULL,
  barbecue_id uuid REFERENCES Barbecue ON UPDATE CASCADE
)

SELECT * FROM barbecue