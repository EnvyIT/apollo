CREATE INDEX IF NOT EXISTS `idx_schedule_start_time`  ON `apollo`.`schedule` (start_time) COMMENT '' ALGORITHM DEFAULT LOCK DEFAULT;
