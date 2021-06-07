# RollbackCheck

Rollback Check
   * comprises of 4 steps:
   * Duplicate all tables specified in <see cref="tables"/>
   * Run PreSchemaCompare and PreSchemaCompareRollback scripts stored in this project
   * Compare all tables specified in <see cref="tables"/> with the backups made, checking for schema and data changes
   * Delete the backup table if the comparison passes, otherwise the backup tables are kept for manual rollback
