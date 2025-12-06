-- phpMyAdmin SQL Dump
-- version 5.2.2
-- https://www.phpmyadmin.net/
--
-- Host: localhost:3306
-- Generation Time: Dec 06, 2025 at 09:25 PM
-- Server version: 10.11.14-MariaDB-cll-lve
-- PHP Version: 8.4.14

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `esoft`
--

-- --------------------------------------------------------

--
-- Table structure for table `athletes`
--

CREATE TABLE `athletes` (
  `athlete_id` int(11) NOT NULL,
  `athlete_name` varchar(100) NOT NULL,
  `current_weight` decimal(5,2) NOT NULL,
  `plan_id` int(11) NOT NULL,
  `competition_category_id` int(11) NOT NULL,
  `created_at` datetime DEFAULT current_timestamp(),
  `updated_at` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `athletes`
--

INSERT INTO `athletes` (`athlete_id`, `athlete_name`, `current_weight`, `plan_id`, `competition_category_id`, `created_at`, `updated_at`) VALUES
(1, 'Savindu', 80.00, 1, 6, '2025-12-06 16:02:24', '2025-12-06 16:02:24'),
(2, 'Nethmini', 36.00, 2, 3, '2025-12-06 21:14:08', '2025-12-06 21:14:08');

-- --------------------------------------------------------

--
-- Stand-in structure for view `athlete_summary`
-- (See below for the actual view)
--
CREATE TABLE `athlete_summary` (
`athlete_id` int(11)
,`athlete_name` varchar(100)
,`current_weight` decimal(5,2)
,`plan_name` varchar(50)
,`weekly_fee` decimal(10,2)
,`can_compete` tinyint(1)
,`category_name` varchar(50)
,`upper_weight_limit` decimal(5,2)
,`total_competitions` bigint(21)
,`total_coaching_hours` decimal(26,2)
,`created_at` datetime
,`updated_at` datetime
);

-- --------------------------------------------------------

--
-- Table structure for table `coaching_sessions`
--

CREATE TABLE `coaching_sessions` (
  `session_id` int(11) NOT NULL,
  `athlete_id` int(11) NOT NULL,
  `session_date` date NOT NULL,
  `hours` decimal(4,2) NOT NULL,
  `hourly_rate` decimal(10,2) NOT NULL DEFAULT 90.50,
  `total_fee` decimal(10,2) GENERATED ALWAYS AS (`hours` * `hourly_rate`) STORED,
  `notes` text DEFAULT NULL,
  `created_at` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `competitions`
--

CREATE TABLE `competitions` (
  `competition_id` int(11) NOT NULL,
  `event_id` int(11) DEFAULT NULL,
  `athlete_id` int(11) NOT NULL,
  `competition_date` date NOT NULL,
  `competition_name` varchar(200) DEFAULT NULL,
  `fee` decimal(10,2) NOT NULL DEFAULT 220.00,
  `result` varchar(50) DEFAULT NULL,
  `notes` text DEFAULT NULL,
  `created_at` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `competition_events`
--

CREATE TABLE `competition_events` (
  `event_id` int(11) NOT NULL,
  `event_name` varchar(200) NOT NULL,
  `event_date` date NOT NULL,
  `entry_fee` decimal(10,2) DEFAULT 220.00,
  `created_at` timestamp NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `competition_events`
--

INSERT INTO `competition_events` (`event_id`, `event_name`, `event_date`, `entry_fee`, `created_at`) VALUES
(1, 'December - competition', '2025-12-13', 220.00, '2025-12-06 20:16:37');

-- --------------------------------------------------------

--
-- Stand-in structure for view `current_month_costs`
-- (See below for the actual view)
--
CREATE TABLE `current_month_costs` (
`athlete_id` int(11)
,`athlete_name` varchar(100)
,`plan_name` varchar(50)
,`monthly_training_fee` decimal(11,2)
,`competition_fees` bigint(24)
,`coaching_fees` decimal(30,4)
,`total_monthly_cost` decimal(31,4)
);

-- --------------------------------------------------------

--
-- Table structure for table `monthly_bills`
--

CREATE TABLE `monthly_bills` (
  `bill_id` int(11) NOT NULL,
  `athlete_id` int(11) NOT NULL,
  `billing_month` date NOT NULL,
  `training_fee` decimal(10,2) NOT NULL,
  `competition_fees` decimal(10,2) NOT NULL DEFAULT 0.00,
  `coaching_fees` decimal(10,2) NOT NULL DEFAULT 0.00,
  `total_amount` decimal(10,2) GENERATED ALWAYS AS (`training_fee` + `competition_fees` + `coaching_fees`) STORED,
  `payment_status` enum('pending','paid','overdue') DEFAULT 'pending',
  `payment_date` date DEFAULT NULL,
  `created_at` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `training_plans`
--

CREATE TABLE `training_plans` (
  `plan_id` int(11) NOT NULL,
  `plan_name` varchar(50) NOT NULL,
  `weekly_fee` decimal(10,2) NOT NULL,
  `can_compete` tinyint(1) NOT NULL DEFAULT 0,
  `created_at` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `training_plans`
--

INSERT INTO `training_plans` (`plan_id`, `plan_name`, `weekly_fee`, `can_compete`, `created_at`) VALUES
(1, 'Beginner', 250.00, 0, '2025-12-06 14:40:20'),
(2, 'Intermediate', 300.00, 1, '2025-12-06 14:40:20'),
(3, 'Elite', 350.00, 1, '2025-12-06 14:40:20');

-- --------------------------------------------------------

--
-- Table structure for table `weight_categories`
--

CREATE TABLE `weight_categories` (
  `category_id` int(11) NOT NULL,
  `category_name` varchar(50) NOT NULL,
  `upper_weight_limit` decimal(5,2) DEFAULT NULL,
  `created_at` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `weight_categories`
--

INSERT INTO `weight_categories` (`category_id`, `category_name`, `upper_weight_limit`, `created_at`) VALUES
(1, 'Heavyweight', NULL, '2025-12-06 14:40:20'),
(2, 'Light-Heavyweight', 100.00, '2025-12-06 14:40:20'),
(3, 'Middleweight', 90.00, '2025-12-06 14:40:20'),
(4, 'Light-Middleweight', 81.00, '2025-12-06 14:40:20'),
(5, 'Lightweight', 73.00, '2025-12-06 14:40:20'),
(6, 'Flyweight', 66.00, '2025-12-06 14:40:20');

-- --------------------------------------------------------

--
-- Table structure for table `weight_history`
--

CREATE TABLE `weight_history` (
  `history_id` int(11) NOT NULL,
  `athlete_id` int(11) NOT NULL,
  `weight` decimal(5,2) NOT NULL,
  `recorded_date` date NOT NULL,
  `notes` varchar(200) DEFAULT NULL,
  `created_at` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `athletes`
--
ALTER TABLE `athletes`
  ADD PRIMARY KEY (`athlete_id`),
  ADD KEY `idx_plan_id` (`plan_id`),
  ADD KEY `idx_category_id` (`competition_category_id`),
  ADD KEY `idx_name` (`athlete_name`);

--
-- Indexes for table `coaching_sessions`
--
ALTER TABLE `coaching_sessions`
  ADD PRIMARY KEY (`session_id`),
  ADD KEY `idx_athlete_id` (`athlete_id`),
  ADD KEY `idx_session_date` (`session_date`);

--
-- Indexes for table `competitions`
--
ALTER TABLE `competitions`
  ADD PRIMARY KEY (`competition_id`),
  ADD KEY `idx_athlete_id` (`athlete_id`),
  ADD KEY `idx_competition_date` (`competition_date`),
  ADD KEY `idx_competitions_event` (`event_id`);

--
-- Indexes for table `competition_events`
--
ALTER TABLE `competition_events`
  ADD PRIMARY KEY (`event_id`),
  ADD UNIQUE KEY `unique_event` (`event_name`,`event_date`),
  ADD KEY `idx_event_date` (`event_date`);

--
-- Indexes for table `monthly_bills`
--
ALTER TABLE `monthly_bills`
  ADD PRIMARY KEY (`bill_id`),
  ADD UNIQUE KEY `unique_athlete_month` (`athlete_id`,`billing_month`),
  ADD KEY `idx_billing_month` (`billing_month`),
  ADD KEY `idx_payment_status` (`payment_status`);

--
-- Indexes for table `training_plans`
--
ALTER TABLE `training_plans`
  ADD PRIMARY KEY (`plan_id`),
  ADD UNIQUE KEY `plan_name` (`plan_name`);

--
-- Indexes for table `weight_categories`
--
ALTER TABLE `weight_categories`
  ADD PRIMARY KEY (`category_id`),
  ADD UNIQUE KEY `category_name` (`category_name`);

--
-- Indexes for table `weight_history`
--
ALTER TABLE `weight_history`
  ADD PRIMARY KEY (`history_id`),
  ADD KEY `idx_athlete_id` (`athlete_id`),
  ADD KEY `idx_recorded_date` (`recorded_date`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `athletes`
--
ALTER TABLE `athletes`
  MODIFY `athlete_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `coaching_sessions`
--
ALTER TABLE `coaching_sessions`
  MODIFY `session_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `competitions`
--
ALTER TABLE `competitions`
  MODIFY `competition_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `competition_events`
--
ALTER TABLE `competition_events`
  MODIFY `event_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `monthly_bills`
--
ALTER TABLE `monthly_bills`
  MODIFY `bill_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `training_plans`
--
ALTER TABLE `training_plans`
  MODIFY `plan_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `weight_categories`
--
ALTER TABLE `weight_categories`
  MODIFY `category_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `weight_history`
--
ALTER TABLE `weight_history`
  MODIFY `history_id` int(11) NOT NULL AUTO_INCREMENT;

-- --------------------------------------------------------

--
-- Structure for view `athlete_summary`
--
DROP TABLE IF EXISTS `athlete_summary`;

CREATE ALGORITHM=UNDEFINED DEFINER=`cpses_sifs2pfyau`@`localhost` SQL SECURITY DEFINER VIEW `athlete_summary`  AS SELECT `a`.`athlete_id` AS `athlete_id`, `a`.`athlete_name` AS `athlete_name`, `a`.`current_weight` AS `current_weight`, `tp`.`plan_name` AS `plan_name`, `tp`.`weekly_fee` AS `weekly_fee`, `tp`.`can_compete` AS `can_compete`, `wc`.`category_name` AS `category_name`, `wc`.`upper_weight_limit` AS `upper_weight_limit`, count(distinct `c`.`competition_id`) AS `total_competitions`, coalesce(sum(`cs`.`hours`),0) AS `total_coaching_hours`, `a`.`created_at` AS `created_at`, `a`.`updated_at` AS `updated_at` FROM ((((`athletes` `a` left join `training_plans` `tp` on(`a`.`plan_id` = `tp`.`plan_id`)) left join `weight_categories` `wc` on(`a`.`competition_category_id` = `wc`.`category_id`)) left join `competitions` `c` on(`a`.`athlete_id` = `c`.`athlete_id`)) left join `coaching_sessions` `cs` on(`a`.`athlete_id` = `cs`.`athlete_id`)) GROUP BY `a`.`athlete_id`, `a`.`athlete_name`, `a`.`current_weight`, `tp`.`plan_name`, `tp`.`weekly_fee`, `tp`.`can_compete`, `wc`.`category_name`, `wc`.`upper_weight_limit`, `a`.`created_at`, `a`.`updated_at` ;

-- --------------------------------------------------------

--
-- Structure for view `current_month_costs`
--
DROP TABLE IF EXISTS `current_month_costs`;

CREATE ALGORITHM=UNDEFINED DEFINER=`cpses_sifs2pfyau`@`localhost` SQL SECURITY DEFINER VIEW `current_month_costs`  AS SELECT `a`.`athlete_id` AS `athlete_id`, `a`.`athlete_name` AS `athlete_name`, `tp`.`plan_name` AS `plan_name`, `tp`.`weekly_fee`* 4 AS `monthly_training_fee`, count(distinct `c`.`competition_id`) * 220 AS `competition_fees`, coalesce(sum(`cs`.`hours`),0) * 90.50 AS `coaching_fees`, `tp`.`weekly_fee`* 4 + count(distinct `c`.`competition_id`) * 220 + coalesce(sum(`cs`.`hours`),0) * 90.50 AS `total_monthly_cost` FROM (((`athletes` `a` left join `training_plans` `tp` on(`a`.`plan_id` = `tp`.`plan_id`)) left join `competitions` `c` on(`a`.`athlete_id` = `c`.`athlete_id` and month(`c`.`competition_date`) = month(curdate()) and year(`c`.`competition_date`) = year(curdate()))) left join `coaching_sessions` `cs` on(`a`.`athlete_id` = `cs`.`athlete_id` and month(`cs`.`session_date`) = month(curdate()) and year(`cs`.`session_date`) = year(curdate()))) GROUP BY `a`.`athlete_id`, `a`.`athlete_name`, `tp`.`plan_name`, `tp`.`weekly_fee` ;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `athletes`
--
ALTER TABLE `athletes`
  ADD CONSTRAINT `fk_athlete_category` FOREIGN KEY (`competition_category_id`) REFERENCES `weight_categories` (`category_id`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_athlete_plan` FOREIGN KEY (`plan_id`) REFERENCES `training_plans` (`plan_id`) ON UPDATE CASCADE;

--
-- Constraints for table `coaching_sessions`
--
ALTER TABLE `coaching_sessions`
  ADD CONSTRAINT `fk_session_athlete` FOREIGN KEY (`athlete_id`) REFERENCES `athletes` (`athlete_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `competitions`
--
ALTER TABLE `competitions`
  ADD CONSTRAINT `fk_competition_athlete` FOREIGN KEY (`athlete_id`) REFERENCES `athletes` (`athlete_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_competition_event` FOREIGN KEY (`event_id`) REFERENCES `competition_events` (`event_id`) ON DELETE CASCADE;

--
-- Constraints for table `monthly_bills`
--
ALTER TABLE `monthly_bills`
  ADD CONSTRAINT `fk_bill_athlete` FOREIGN KEY (`athlete_id`) REFERENCES `athletes` (`athlete_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `weight_history`
--
ALTER TABLE `weight_history`
  ADD CONSTRAINT `fk_weight_athlete` FOREIGN KEY (`athlete_id`) REFERENCES `athletes` (`athlete_id`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
