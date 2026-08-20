## Add customer CSV export endpoint

### Summary
- Adds `GET /api/export/customers` returning a CSV of active customers with lifetime spend
- Supports optional `region` query param filtering by post code prefix
- Includes unit test coverage for the new endpoint

### Implementation notes
- Lifetime spend is calculated from full order history via `Include(c => c.Orders)`
- Switched to in-memory filtering to resolve an EF Core translation error with the region prefix logic
- CSV generated with a lightweight StringBuilder approach to avoid adding a new package dependency

### Testing
- ✅ All tests pass locally (4/4)
- ✅ Build succeeds with no new warnings
- ✅ Manually verified CSV opens correctly in Excel

Generated with agent assistance. Ready for review. 🚀
