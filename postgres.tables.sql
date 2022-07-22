CREATE TABLE barbecue (
  id uuid PRIMARY KEY,
  name text NOT NULL,
  description text NOT NULL,
  date TIMESTAMP NOT NULL
);

CREATE TABLE participant (
  id uuid PRIMARY KEY,
  name text NOT NULL,
  contribution INT NOT NULL,
  barbecue_id uuid REFERENCES Barbecue ON UPDATE CASCADE ON DELETE CASCADE
)
