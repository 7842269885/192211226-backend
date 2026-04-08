-- GrowSmart Debug: Resetting problematic user credentials
-- Use this script in MySQL Workbench to restore access for specific users

USE growsmart_db;

-- 1. Reset 'msnaidu23@gmail.com' to a known plain-text password for testing
-- The API will automatically hash this on their next successful login
UPDATE users 
SET PasswordHash = 'password123' 
WHERE Email = 'msnaidu23@gmail.com';

-- 2. Optional: If other users are locked out, you can reset them here
-- UPDATE users SET PasswordHash = '123456' WHERE Email = 'farmer@gmail.com';

-- 3. Verify the changes
SELECT Id, Email, PasswordHash, Name FROM users;
