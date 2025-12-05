# Oven Log Application - Feature Requirements List

This document contains a comprehensive feature requirement list derived from the Oven Log User Guide, including all text content and visual elements (screenshots, database schema diagrams).

---

## 1. Overall System Goals and Deployment

1.1. The application must functionally replace existing paper oven logs.

1.2. The application must support multiple users sharing a single, central database instance (no local/private copies of the data).

1.3. Users must be able to:
   - Enter a TRAK ID
   - Select an oven
   - Modify the oven's default temperature
   - Select an application
   - Modify the application's default bake time
   - Modify the current date/time
   - Set TRAK quantity
   - Add the TRAK to an oven

1.4. The system must maintain and display a current list of all TRAKs in ovens, including their remaining bake time.

1.5. Users must be able to remove TRAKs from ovens via:
   - TRAK ID input
   - List selection
   - Scanning a dedicated "Remove" action barcode

1.6. The system must support operation with a barcode reader that appends a Tab-character autoterminator to each scan.

1.7. In a multi-user environment, all users must operate against the same central data so oven status is consistent across stations.

---

## 2. Data Model and Entities

### 2.1 Locations (tblLocation)
- Maintain a tblLocation entity with fields:
  - `bytLocation` (key)
  - `strLocation` (human-readable name)
- Each oven/equipment "box" record must reference a location via `bytLocation`.

### 2.2 Types (tblType)
- Maintain a tblType entity with fields:
  - `bytType` (key)
  - `strType` (e.g., oven, freezer, refrigerator, solder pot, dry box, cart, etc.)
- Each box record must reference a type via `bytType`.

### 2.3 Manufacturers (tblMfg)
- Maintain a tblMfg entity with fields:
  - `bytMfg` (key)
  - `strMfg` (manufacturer name)

### 2.4 Models (tblModel)
- Maintain a tblModel entity with fields:
  - `lngModel` (key)
  - `strModel` (model name)
  - `bytMfr` (FK to manufacturer)
- Each box must reference a model via `lngModel`, thereby indirectly associating a manufacturer.

### 2.5 Boxes/Equipment (tblBox)
- Maintain a tblBox entity representing devices such as ovens, freezers, solder pots, refrigerators, dry boxes, carts, etc.
- Required fields:
  - `strID` (tool identifier)
  - `bytLocation` (FK to Location)
  - `lngModel` (FK to Model)
  - `bytType` (FK to Type)
  - `lngBox` (key)
  - `dblTemp` (default temperature)
  - `strScale` (temperature unit/scale, e.g., "C" for Celsius)
  - `dblWarm` (warm-up time in minutes/hours)
- Each box record must be associated with one Type, one Location, and one Model.
- Warm-up time (`dblWarm`) must be used by business logic for warm-up enforcement on ovens without digital displays.

### 2.6 Parts (tblPart)
- Maintain a tblPart entity with fields:
  - `lngPart` (key)
  - `strPart` (part number)
  - `strPartDesc` (description)

### 2.7 TRAKs (tblTrak)
- Maintain a tblTrak entity with fields:
  - `lngTrak` (key)
  - `strTrak` (TRAK identifier)
  - `strSerial` (serial number)
  - `strWO` (work order)
  - `lngPart` (FK to Part)
- When a TRAK ID is scanned or manually entered, Part and TRAK records must be created/updated based on data from the external system CIMA.

### 2.8 Users (tblUser)
- Maintain a primary tblUser entity with fields:
  - `lngUser` (key)
  - `strFirst` (first name)
  - `strMiddle` (middle name)
  - `strLast` (last name)
  - `strBadge` (badge number)
  - `strLogin` (login identifier)
  - `boolActive` (active/inactive flag)
  - `lngBox` (optional FK to Box for dedicated-station oven association)
- Users must be markable as active/inactive via `boolActive`.
- User badge (`strBadge`) and login (`strLogin`) values must be used for identity and linking to external systems.

### 2.9 User Aliases (tblAlias)
- The data model must support storing "alias" records that map different login/identifier values to one physical person.
- Alias fields must include at least:
  - `strAlias` (the alias identifier, typically email/old login)
  - `strUserName` (the new/current login)
- Business rules for aliases:
  - When a user's login changes but their email address remains the same, their email address (old login) should appear both in `tblUser.strLogin` and in `tblAlias.strAlias`, while their new login must be stored in `tblAlias.strUserName`.
  - The email address tends to be the simple/original login (e.g., first.last), whereas new logins may be more complex (e.g., first.m.last or first.last2).

### 2.10 Descriptions/Standard Times (tblDesc)
- Maintain a tblDesc entity with fields:
  - `lngDesc` (key)
  - `strBar` (barcode text)
  - `strDesc` (human description)
  - `dblHours` (standard hours)
- tblDesc values must be used to define standard times and barcodes that appear on the Customize/Barcode form.

### 2.11 Events (tblEvent)
- Maintain a tblEvent entity in which each record represents an oven event, containing both "in" and "out" aspects.
- Required fields:
  - `lngEvent` (key)
  - `lngBox` (FK to box/oven)
  - `lngUserIn` (FK to user who added TRAK)
  - `datIn` (date/time TRAK was added to oven)
  - `lngUserOut` (FK to user who removed TRAK)
  - `datOut` (date/time TRAK was removed from oven)
  - `lngTrak` (FK to TRAK)
  - `memNote` (notes entered at add time)
  - `lngQty` (quantity)
  - `lngDesc` (FK to description/standard time)
  - `dblHours` (bake time in hours)
  - `dblTemp` (bake temperature)
- `datIn`/`dblHours`/`dblTemp` must be used to calculate and display bake-time remaining and actual bake time.

### 2.12 Turn-On Events (tblOn)
- Maintain a tblOn entity for "oven on" events, with fields:
  - `lngOn` (key)
  - `lngBox` (FK to box)
  - `lngUser` (user who turned oven on)
  - `datOn` (intended start time)
  - `datActual` (actual recorded time)
- These records must only be created and used for ovens without digital temperature displays.

---

## 3. Main Form - General Behavior and Modes

### 3.1 Operating Modes
- The main form must operate in two modes: **Barcode Mode** and **Manual Mode**.
- **Barcode Mode** must be the default and "preferred" mode whenever the form is reset.
- In **Barcode Mode**, all keystrokes arriving at the form (from keyboard or barcode scanner) must be intercepted and redirected to the appropriate field based on content:
  - TRAK IDs go to the TRAK ID box / TRAK list input
  - Oven identifiers go to the oven selection list/dropdown
  - Application identifiers go to the application dropdown/list
  - Bake time barcodes go to the bake time field (hh:mm)
  - All other input goes to the Notes field
- In **Manual Mode**, keystrokes must behave normally, going to whichever control currently has focus; no interception/routing occurs.

### 3.2 Mode Toggle Controls
- The main form must provide a control labeled "Unlock for manual entry" to switch to Manual Mode.
- The main form must provide a control labeled "Lock for barcode entry" to switch back to Barcode Mode.
- The mode label (e.g., "BARCODE MODE" / "MANUAL MODE") must be visually indicated at the top of the form.

### 3.3 Barcode Reader Integration
- The system must rely on the barcode reader's Tab autoterminator to signal end of each scanned token, allowing routing logic to act on complete values.

---

## 4. Main Form - Entry Fields

### 4.1 Required Top-Row Fields
The main form must provide these fields and controls:
- **TRAK ID input** - Text field for entering/scanning TRAK identifiers
- **Oven dropdown** - Selector for choosing the target oven (e.g., showing oven ID like "110945")
- **Temperature field** - Numeric field with units displayed (e.g., "105 °C")
- **Start Time field** - Date/time picker (e.g., "8/10/2012 6:07:20 PM")
- **Qty (quantity) field** - Numeric field for TRAK quantity (e.g., "1")
- **Application dropdown** - List of applications (e.g., "Hysol", "Eccobond")
- **Notes field** - Free-text field for additional notes
- **Bake time field** - Time field formatted as hh:mm (e.g., "0:30")

### 4.2 Default Value Population
- For ovens with default temperature, selecting the oven must prepopulate the temperature field with the default, while still allowing user override.
- For applications with default times, selecting the application must prepopulate the bake time field with the default, while still allowing user override.

---

## 5. TRAK List (Left Panel)

### 5.1 Display Requirements
- The left side of the main form must display a TRAK list.
- The list must display at least the TRAK ID and quantity for each pending TRAK.

### 5.2 Population Methods
The TRAK list must be populated by either:
- Scanning a TRAK ID barcode, or
- Typing a TRAK ID into the TRAK ID text box and pressing Tab or Enter

### 5.3 TRAK Validation Logic
- If the TRAK ID does not appear in the TRAK list after entry, the system must handle:
  - The TRAK is not an active work order TRAK (do not add to list)
  - The TRAK is already in an oven (select the corresponding record in the Oven list instead)

### 5.4 Multi-Selection and Batch Mode
- The TRAK list must support multi-selection of TRAKs for batch addition to ovens.
- In batch mode, all selected TRAKs must be assigned the same oven, temperature, start time, bake time, application, quantity, and note when added.
- Multiple TRAKs of the same work order can be selected by scanning or clicking.

### 5.5 Check Button (TRAK List)
- A Check button on the main form must select or deselect all TRAKs in the TRAK list.

---

## 6. Oven List (Right Panel)

### 6.1 Display Requirements
- The right side of the main form must display the Oven list, representing all TRAKs currently in ovens.
- For each TRAK-in-oven record, these columns must be shown:
  - **Oven** - Oven identifier
  - **Location** - Physical location (e.g., "F2")
  - **PN** - Part Number (e.g., "PN001")
  - **SN** - Serial Number (e.g., "SN01")
  - **TRAK** - TRAK identifier (e.g., "TRK02")
  - **Q** - Quantity
  - **Temp** - Temperature (e.g., "65 °C")
  - **Badge** - Badge number of the user who added the TRAK
  - **In** - Oven-in Date/Time (e.g., "8/10 6:06 PM")
  - **Application** - Application name (e.g., "Eccobond")
  - **Time** - Time remaining (e.g., "1:58" or "1:59")
  - **Note** - Notes entered when TRAK was added

### 6.2 Selection and Interaction
- The list must support multiple selection of TRAKs for operations like Remove and History.
- The Oven list must be scrollable to handle many entries.
- The Oven list must visually highlight selected rows.

### 6.3 Time Remaining Calculation
- Time remaining must be derived from bake time vs current time.
- Time remaining must be recalculable on demand via the Reset button.

---

## 7. Main Form Actions and Buttons

### 7.1 Add Button
- The Add button must create oven-event records for the selected TRAK(s) and move them into the Oven list.
- **Minimum required inputs** before enabling Add:
  - At least one TRAK selected
  - An oven selected
  - A bake temperature
  - A quantity
  - A start time
  - A bake time
- Application and Note are optional but should be stored if provided.
- When multiple TRAKs are selected, Add must create corresponding entries so that each selected TRAK gets the same oven, temperature, start time, bake time, application, quantity, and note.
- Add must write a record into tblEvent for each TRAK.

### 7.2 History Button
- With one or more TRAKs selected in either the TRAK list and/or the Oven list, clicking History must display all oven events related to those TRAKs.
- The History display must include enough information to understand past oven-in/oven-out activity per TRAK.
- The purpose of History must include enabling "undo" investigation when a TRAK is accidentally removed from the Oven list.

### 7.3 Remove Button
- With one or more TRAKs selected in the Oven list, clicking Remove must:
  - Remove those entries from the visible Oven list
  - Close out their associated event records (set oven-out date/time and user)
- **Premature Removal Warning**: If there is still time remaining on any selected TRAK's bake, the system must prompt the user with a confirmation dialog asking if they are sure they want to remove the TRAK(s) prematurely.
- **Actual Bake Time Definition**: The system must frame "actual bake time" as the time the record spent on the Oven list (virtual oven), which may exceed the nominal bake time and the physical bake duration.

### 7.4 Reset Button
The Reset button must:
- Clear the TRAK list
- Deselect all entries in the Oven list
- Refresh the remaining-time calculations in the Oven list
- Clear all other top-row controls (TRAK ID, oven, temperature, start time, qty, application, notes, bake time)
- Return the form to Barcode Mode as the default mode for keystroke interception

### 7.5 Check Button (Main Form)
- The Check button must serve as a toggle that selects or deselects all TRAKs currently visible in the TRAK list.

### 7.6 Customize Button
- The Customize button on the main form must open the Customize Form where users can configure ovens, applications, and standard time barcodes to appear on a printable barcode sheet.

### 7.7 Oven On Button
- With an oven selected in the oven dropdown, clicking Oven On must log an oven "turn-on" event in tblOn using the time displayed in the Start Time field.
- The Oven On button must only function for ovens without digital temperature displays; it must be disabled or logically ignored for ovens that do have digital displays.
- **Warm-Up Enforcement**: Ovens that require warm-up must have a labeled minimum warm-up time, and the application must enforce that a TRAK cannot be added to an oven that is currently undergoing warm-up based on its warm-up time and last "Oven On" event.

### 7.8 Tools Button
- The Tools button must open the Tools Form, which provides administrative/maintenance and reporting capabilities.

### 7.9 Exit Button
- The Exit button must close the Oven Log application cleanly.

---

## 8. Customize Form

### 8.1 General Layout
The Customize Form must present at least three lists:
- An **Oven/Equipment list** (left side)
- An **Applications list** (center/right)
- A **Standard Times list** (rightmost)

Bottom buttons must include:
- Go Back
- Barcode Sheet
- Show Selection
- Save Selection
- Check (for selecting all ovens)

### 8.2 Oven List (Customize Form)
- This list must show all devices from tblBox.
- Columns must include:
  - **Type** (from tblType, e.g., "Cart", "Dry Box", "Oven")
  - **Manufacturer** (from tblMfg via model, e.g., "Erecta", "McDry", "Sheldon", "Despatch", "Blue M", "Precision", "Associated", "Dr. Storage")
  - **Model** (from tblModel, e.g., "Shelf", "DXU-1002-10", "X2E-480", "MCU-580SE", "X2M-600", "BK1104", "FX14-2", "LFD2-24", "OV-490A-2", "LFD1-42", "29")
  - **Tool ID** (from tblBox.strID)
  - **Application/Usage** (if available)
  - **Temperature** (default temperature, e.g., "22 °C", "65 °C", "80 °C", "105 °C", "225 °F")
  - **Warm-up time** (if any, e.g., "0:00", "0:10", "3:30")
- Users must be able to select/deselect multiple ovens in this list.
- The Check button on this form must select or deselect all ovens.

### 8.3 Application List (Customize Form)
- The application list must list all applications from the Applications master data.
- Columns must include:
  - **Application Name**
  - **Default Bake Time** (e.g., "5:00", "2:00", "24:00", "48:00", "0:15", "0:30", "4:00")
  - **Temperature or Temperature Range** (e.g., "102 to 108 °C", "20 to 25 °C", "74 to 80 °C", "77 to 83 °C", "105 °C", "65 °C", "77 °C", "100 to 110 °C", "65 to 105 °C", "62 to 68 °C")
  - **Additional time column** (if applicable)

Example applications that must be representable:
- Ablefilm, Air Cure 2, Air Cure 24, Air Cure 48, Air Cure 8
- Arathane 2, Arathane 8, Armstrong C7, C7 CAB-O-SIL
- Chipbond 3607, Chipbond 3621, Conap, Dymax, Eccobond
- Enthone Ink, Humiseal 1B31, Humiseal 2A64, Hysol
- Label, Lubricant, Mask/Prebake, Prebake
- Resiweld, RTV, Scotchweld, Solder Mask

### 8.4 Time List (Standard Times)
- The Time list must display all standard bake times defined in tblDesc/Standard Times configuration.
- Standard times visible in the screenshot include:
  - 0:03, 0:10, 0:15, 0:30, 1:00, 1:30, 2:00, 3:00, 3:30, 4:00, 4:30, 5:00, 5:30, 6:00, 7:00, 7:30, 8:00, 12:00, 24:00, 48:00, 72:00
- Users must be able to select standard time entries that will appear as time barcodes on the barcode sheet.

### 8.5 Show Selection Button
- Show Selection must filter/mark ovens that are already associated with the current user as their preferred ovens.
- When invoked, only these ovens must be visible on the main form for that user.

### 8.6 Save Selection Button
- Save Selection must persist the currently selected ovens in the oven list as the user's preferred ovens.
- After saving, only those ovens are visible/available in the main form for add/remove operations.

### 8.7 Barcode Sheet Button
- The Barcode Sheet button must generate a printable report that includes barcodes for the selected ovens, applications, and standard times.
- Each printed page must be able to hold up to **eleven ovens, eleven applications, and eleven time entries**.
- Each barcode sheet page must include the **four action barcodes**: Reset, Oven On, Add, and Remove.
- The font and barcode rendering must use the proper barcode font (e.g., v100001_.ttf or equivalent) so that barcodes scan correctly.
- If the font is missing, the system must provide guidance to install it (drag v100001_.ttf to C:\WINDOWS\Fonts).

---

## 9. Barcode Behavior

### 9.1 Barcode Compatibility
- All data-entry barcodes (TRAKs, ovens, applications, standard times, action barcodes) must be compatible with the system's Barcode Mode routing logic and Tab-autoterminating scanners.

### 9.2 Action Barcodes
- Action barcodes such as Reset, Oven On, Add, and Remove must trigger the same behaviors as their corresponding on-screen buttons when scanned.

### 9.3 Standard Time Barcodes
- Standard time barcodes must populate the bake time field with the associated time value.
- The system must interpret barcodes attached to descriptions in tblDesc (strBar) to route either to the time field or appropriate controls.

---

## 10. Tools Form - Administration and Utilities

### 10.1 Overall Layout
The Tools Form must present buttons organized into two columns:

**Left Column:**
- Manufacturer > Model
- Models
- Types
- Locations
- Ovens, Dryboxes, etc
- User > Alias
- Applications
- Go Back

**Right Column:**
- TRAKs for Testing
- Standard Times
- View Entire Database
- Recent Activity
- TRAKs In Ovens

A barcode image (decorative or functional) must appear in the upper-right area.

### 10.2 Manufacturer > Model Button
- This control must open a view allowing editing of the manufacturer table and its indented model table (master-detail).
- Users must be able to add, edit, and deactivate manufacturers and models.

### 10.3 Models Button
- This control must allow maintenance of models of ovens, dry boxes, and similar equipment, independent of manufacturer context when needed.

### 10.4 Types Button
- This control must open the types table (box types) for editing values such as oven, freezer, refrigerator, dry box, cart, etc.

### 10.5 Locations Button
- This control must open the locations table for editing, supporting addition and modification of locations where ovens and equipment reside.

### 10.6 Ovens, Dryboxes, etc Button
- This control must open the boxes table editor, enabling administrators to add or edit ovens, dryboxes, and other equipment records.
- Editable fields must include: model, type, location, tool ID, default temperature, and warm-up time.

### 10.7 User > Alias Button
- This control must open the user table with an indented alias table for editing.
- It must support:
  - Editing user details (first, middle, last name, badge, login, active flag, dedicated-oven association via lngBox when applicable)
  - Adding alias records that map alternative login names or badges to the same physical user
- **RelinkUser Operation**: If a user's login changes and they open Oven Log, a second user record might be created. The correct handling is:
  - Relink child records associated with the new user record to the original user record
  - Add an Alias record for the new login, linking it to the original user
  - Delete the duplicate/new user record
- The system must provide a way to perform or support the "RelinkUser" operation.

### 10.8 Applications Button
- This control must open the applications table, allowing maintenance of application records and their default bake times (and default temperatures).
- Applications must be editable such that they appear both in the main form application dropdown and in the Customize Form list.

### 10.9 TRAKs for Testing Button
- This control must produce a report showing **63 TRAK ID barcodes** designed for testing the interface.
- The report must be printable and the barcodes must be scannable by the same barcode-reading and routing logic used in production.

### 10.10 Standard Times Button
- This control must open the configuration interface for editing and creating standard bake times that appear in the Time list on the Customize Form and as standard-time barcodes.
- Each standard time must support editing of time duration and possibly associated description or barcode string.

### 10.11 View Entire Database Button
- This control must open a query or view that shows all information about all oven events recorded in tblEvent.
- Information must include: TRAK, oven, users in/out, in/out times, temperatures, quantities, notes, and descriptions.
- The view must be suitable for auditing and export.

### 10.12 Recent Activity Button
- This control must open a view similar to "View Entire Database" but filtered to only show events from the **last 24 hours**.
- The 24-hour window must be computed relative to current time.

### 10.13 TRAKs In Ovens Button
- This control must open a view that shows the same detailed event information as "View Entire Database" but limited to only those TRAKs currently displayed in the Oven list on the main form.

---

## 11. Dedicated Stations and MAC Address Configuration

### 11.1 MAC Address Table
- The system must maintain a MAC Address-related table for station identification.
- Historically, a "What's my MAC?" function allowed a user to retrieve the station's MAC address, then add it to the MAC Address table and specify an oven index (lngBox) to make the station dedicated to that oven.

### 11.2 Current Dedicated Station Logic
- Due to identical MAC addresses on thin clients, the effective current requirement is: a station is "dedicated" if the logged-in user's record in tblUser has a non-null `lngBox` (oven index).

### 11.3 Dedicated Station Behavior
For **dedicated stations**:
- The main form must disable the oven and temperature combo boxes
- An Operator combo box must be enabled, allowing operators to be selected

For **non-dedicated stations**:
- The oven and temperature combo boxes must be enabled
- The Operator combo box must be disabled or hidden

### 11.4 What's My MAC? Button
- The Tools area must support viewing/editing the MAC Address table (even if its role is reduced).
- The system must support editing user.lngBox to assign or clear dedicated ovens.

---

## 12. Missing Badges Feature

### 12.1 CIMA Integration for Badge Discovery
- The Missing Badges function must query the external CIMA system for THOR labor charged during the **last three months**.
- The function must identify which badges are missing from the Oven Log's local user table.

### 12.2 Badge Processing Rules
For each badge found:
- If the termination date in CIMA is **1/1/1901** (indicating an active employee), and the badge is not present in Oven Log, the system must indicate that the user should be added under User > Alias.
- If the physical person already exists under a different badge:
  - The old badge must be moved into the alias table
  - The new badge should become the main badge in the user table

### 12.3 Results Display
- The function must present results in a way that clearly differentiates active employees missing from the user table from others.

---

## 13. External Integrations

### 13.1 CIMA Integration - TRAK/Part Data
- When a TRAK is scanned or manually entered, the system must integrate with CIMA to populate or validate tblPart and tblTrak entries (part and TRAK information).

### 13.2 CIMA Integration - Badge Data
- The Missing Badges function must integrate with CIMA to pull THOR labor data (at least badge and termination date) for the last three months.

### 13.3 Email/Login Aliasing
- The system must support email-related aliasing where some logins are inherited from other SMTP-sending applications.
- Alias records must allow aligning these logins with Oven Log's user records.

---

## 14. Security and Trust Configuration

### 14.1 Initial Setup Requirements
- New machines/stations may require initial trust or configuration.
- For the original MS Access implementation, this involved adding the network path to Access Trusted Locations:
  - Select Options when Security Warning appears
  - Click "Open the Trust Center"
  - Click "Trusted Locations" > "Add new location"
  - Type the path (e.g., "P:\Access") in the Path field
  - Check "Subfolders of this location are also trusted"
  - Click OK > OK > OK
  - Close and re-open Oven Log
- For a modern implementation, a similar trust/security configuration or clear installation steps must be defined.

---

## 15. User Interface Requirements

### 15.1 Efficient Barcode Operation
- The UI must support efficient operation via barcode scanning, minimizing mouse/keyboard interaction when in Barcode Mode.

### 15.2 Mode Indication
- The application should default to Barcode Mode.
- The application must provide clear visual cues when in Manual Mode to avoid confusion.

### 15.3 Visual Highlighting
- Selected items in both TRAK list and Oven list must be visually highlighted.
- Different background colors should distinguish between the two lists (e.g., pink/salmon for TRAK list, cyan/light blue for Oven list as shown in screenshots).

### 15.4 Button Layout
- Main form buttons must be arranged horizontally at the bottom:
  - Check (checkmark icon), Add, History, Remove, Reset, Customize, Oven On, Tools, Exit
- Button styling should clearly indicate primary actions (e.g., Add in blue, Remove in red/highlighted).

---

## 16. Business Rules Summary

### 16.1 TRAK Addition Rules
- A TRAK cannot be added to an oven without: TRAK selection, oven selection, temperature, quantity, start time, and bake time.
- A TRAK that is already in an oven cannot be added again; it must be selected in the Oven list instead.
- A TRAK cannot be added to an oven that is currently undergoing warm-up.

### 16.2 TRAK Removal Rules
- Removing a TRAK with remaining bake time requires user confirmation.
- "Actual bake time" is defined as time spent in the virtual oven list, not physical bake time.

### 16.3 Oven Warm-Up Rules
- Ovens without digital temperature displays require "Oven On" logging before TRAKs can be added.
- The warm-up time must elapse before TRAKs can be added to such ovens.

### 16.4 User Preference Rules
- Users can save preferred oven selections that filter the main form view.
- Only preferred ovens are visible/available for add/remove operations after selection is saved.

### 16.5 Centralized Data Rule
- Everyone must use the central version of the database.
- Local copies must not be made.

---

## 17. Open Items / Information Gaps

The following details were not fully specified in the user guide and may require clarification:

17.1. Exact barcode symbology (Code 39, Code 128, etc.) and full set of barcode formats/text encodings used.

17.2. Exact formats and content for the History, View Entire Database, Recent Activity, TRAKs In Ovens, and TRAKs for Testing reports (general purpose and some filters are known but not precise column sets or sort orders).

17.3. Full behavior of the MAC Address table and "What's my MAC?" button in the current version.

17.4. Detailed rules for what constitutes an "active work order TRAK" and how that is determined from CIMA or other data sources.

17.5. Exact rules for when a box is considered to have a "digital temperature display" (and thus when Oven On is disabled).

17.6. Concurrency rules, locking behavior, or conflict resolution when multiple users attempt to add/remove the same TRAK simultaneously.

17.7. Precise constraints/validation on fields such as maximum note length, allowed temperature ranges, allowed bake time ranges, and quantity limits.

---

*Document generated from OvenLogUserGuide.pdf analysis including all text content and visual elements (database schema diagram, main form screenshots, customize form screenshot, and tools form screenshot).*
