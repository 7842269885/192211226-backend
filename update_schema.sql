-- FINAL SCHEMA FIX: Run these 3 lines one by one if they don't already exist.
ALTER TABLE plants ADD COLUMN LastWateredAt DATETIME NULL;
ALTER TABLE plants ADD COLUMN LastFertilizedAt DATETIME NULL;
ALTER TABLE plants ADD COLUMN HealthStatus VARCHAR(50) NOT NULL DEFAULT 'Healthy';
