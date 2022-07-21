CREATE TABLE Barbecue (
  id uuid PRIMARY KEY,
  name text NOT NULL,
  description text NOT NULL,
  date TIMESTAMP NOT NULL
);

CREATE TABLE Participant (
  id uuid PRIMARY KEY,
  name text NOT NULL,
  contribution INT NOT NULL,
  barbecue_id uuid REFERENCES Barbecue ON UPDATE CASCADE ON DELETE CASCADE
)


DROP TABLE participant

SELECT barbecue.id,
  barbecue.name,
  barbecue.description,
  barbecue.date,
  SUM(participant.contribution) as barbecueCost,
  participant.id as participant_id,
  participant.name as participant_name,
  participant.contribution as participant_contribution
FROM barbecue
  INNER JOIN participant ON participant.barbecue_id = barbecue.id
WHERE barbecue.id = '7b203114-84fb-4931-87ff-5b5e806e2716'
GROUP BY barbecue.id, participant.id


SELECT * FROM participant
SELECT * from barbecue

INSERT INTO barbecue(id, name, description, date)
                VALUES (@barbecueId, @name, @description, @date)
RETURNING id;


INSERT INTO participant(id, name, contribution, barbecue_id)
VALUES(gen_random_uuid(), @name, @contribution, @barbecue_id)

SELECT DISTINCT ON (barbecue.id)
 barbecue.id,
  barbecue.name,
  barbecue.description,
  barbecue.date,
  coalesce(SUM(participant.contribution) over(partition by barbecue.id), 0) as barbecueCost,
  COUNT(participant.id) over(partition by barbecue.id) as barbecue_participants
FROM barbecue
  LEFT JOIN participant ON participant.barbecue_id = barbecue.id
WHERE barbecue.id = '7b203114-84fb-4931-87ff-5b5e806e2716'
GROUP BY barbecue.id, participant.id

DELETE FROM barbecue
WHERE barbecue.id = 'd4c420c4-0d52-41e2-8664-89d1a1d3cf89'