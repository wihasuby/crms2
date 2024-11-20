-- Drop deprecated table
DROP TABLE IF EXISTS purchase_history;
DROP TABLE IF EXISTS customers;


-- Create customers table
CREATE TABLE IF NOT EXISTS customers (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    email TEXT UNIQUE NOT NULL,
    phone_number TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create purchase_history table
CREATE TABLE IF NOT EXISTS purchase_history (
    id SERIAL PRIMARY KEY,
    customer_id INTEGER,
    purchasable TEXT NOT NULL,
    price REAL NOT NULL,
    quantity INTEGER NOT NULL,
    total REAL GENERATED ALWAYS AS (price * quantity) STORED,
    purchase_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (customer_id) REFERENCES customers(id) ON DELETE CASCADE
);


-- Drop existing view if it exists
DROP VIEW IF EXISTS MonthlyCustomerReport;

-- Create the MonthlyCustomerReport view
CREATE OR REPLACE VIEW MonthlyCustomerReport AS
  SELECT
    EXTRACT(YEAR FROM purchase_date) AS Year,
    EXTRACT(MONTH FROM purchase_date) AS Month,
    AVG(total) AS AverageSpend,
    SUM((total / 10)::INTEGER) AS TotalLoyaltyPoints
  FROM
    purchase_history
  GROUP BY
    EXTRACT(YEAR FROM purchase_date),
    EXTRACT(MONTH FROM purchase_date)
  ORDER BY
    Year ASC,
    Month ASC;