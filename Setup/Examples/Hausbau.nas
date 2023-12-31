<Project Application="NAS" Version="1.0" SaveDate="2011-12-18">
  <Name>Hausbau</Name>
  <Description>Testprojekt Haus</Description>
  <StartDate>2012-01-02</StartDate>
  <DataDate>2012-01-02</DataDate>
  <Calendars>
    <Calendar>
      <ID>0</ID>
      <Name>Neuer Kalender</Name>
      <Sunday>False</Sunday>
      <Monday>True</Monday>
      <Tuesday>True</Tuesday>
      <Wednesday>True</Wednesday>
      <Thursday>True</Thursday>
      <Friday>True</Friday>
      <Saturday>False</Saturday>
    </Calendar>
  </Calendars>
  <StandardCalendar>0</StandardCalendar>
  <WBS>
    <Number>H</Number>
    <Name>Haus</Name>
    <Items>
      <WBSItem>
        <Number>0</Number>
        <Name>KG</Name>
      </WBSItem>
      <WBSItem>
        <Number>2</Number>
        <Name>OG</Name>
        <Items>
          <WBSItem>
            <Name>R</Name>
          </WBSItem>
          <WBSItem>
            <Name>A</Name>
          </WBSItem>
        </Items>
      </WBSItem>
      <WBSItem>
        <Number>3</Number>
        <Name>DG</Name>
      </WBSItem>
    </Items>
  </WBS>
  <Resources>
    <MaterialResource>
      <Unit>m³</Unit>
      <Name>Beton</Name>
      <CostsPerUnit>100,00</CostsPerUnit>
    </MaterialResource>
    <WorkResource>
      <Name>Karl</Name>
      <CostsPerUnit>35</CostsPerUnit>
      <Limit>8</Limit>
    </WorkResource>
    <WorkResource>
      <Name>Otto</Name>
      <CostsPerUnit>14,00</CostsPerUnit>
      <Limit>4</Limit>
    </WorkResource>
    <WorkResource>
      <Name>Kurt</Name>
      <CostsPerUnit>40,00</CostsPerUnit>
      <Limit>1</Limit>
    </WorkResource>
  </Resources>
  <Activities>
    <Activity>
      <OriginalDuration>10</OriginalDuration>
      <ID>A0010</ID>
      <Name>Aushub Baugrube</Name>
      <EarlyStartDate>2012-01-03</EarlyStartDate>
      <LateStartDate>2012-01-03</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>2</OriginalDuration>
      <ID>A0020</ID>
      <Name>Fundamente betonieren</Name>
      <EarlyStartDate>2012-01-17</EarlyStartDate>
      <LateStartDate>2012-01-17</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>1</OriginalDuration>
      <ID>A0030</ID>
      <Name>Bodenplatte dämmen</Name>
      <EarlyStartDate>2012-01-19</EarlyStartDate>
      <LateStartDate>2012-01-19</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>2</OriginalDuration>
      <ID>A0040</ID>
      <Name>Bodenplatte betonieren</Name>
      <EarlyStartDate>2012-01-20</EarlyStartDate>
      <LateStartDate>2012-01-20</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>10</OriginalDuration>
      <ID>A0050</ID>
      <Name>Mauerwerk Keller</Name>
      <EarlyStartDate>2012-01-24</EarlyStartDate>
      <LateStartDate>2012-01-24</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>2</OriginalDuration>
      <ID>A0060</ID>
      <Name>Deckenplatte Keller</Name>
      <EarlyStartDate>2012-02-07</EarlyStartDate>
      <LateStartDate>2012-02-07</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>10</OriginalDuration>
      <ID>A0070</ID>
      <Name>Tragendes Mauerwerk</Name>
      <EarlyStartDate>2012-02-09</EarlyStartDate>
      <LateStartDate>2012-02-09</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>2</OriginalDuration>
      <ID>A0080</ID>
      <Name>Deckenplatte</Name>
      <EarlyStartDate>2012-02-23</EarlyStartDate>
      <LateStartDate>2012-02-23</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>5</OriginalDuration>
      <ID>A0090</ID>
      <Name>Nichttragendes Mauerwerk</Name>
      <EarlyStartDate>2012-02-27</EarlyStartDate>
      <LateStartDate>2012-02-27</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>3</OriginalDuration>
      <ID>A0100</ID>
      <Name>Mauerwerk / Ringanker</Name>
      <EarlyStartDate>2012-02-27</EarlyStartDate>
      <LateStartDate>2012-02-28</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>1</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>5</OriginalDuration>
      <ID>A0110</ID>
      <Name>Dachkonstruktion</Name>
      <EarlyStartDate>2012-03-01</EarlyStartDate>
      <LateStartDate>2012-04-04</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>24</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>3</OriginalDuration>
      <ID>A0120</ID>
      <Name>Dachdeckung</Name>
      <EarlyStartDate>2012-03-08</EarlyStartDate>
      <LateStartDate>2012-04-11</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>24</TotalFloat>
      <FreeFloat>24</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>5</OriginalDuration>
      <ID>A0130</ID>
      <Name>Fenster</Name>
      <EarlyStartDate>2012-02-23</EarlyStartDate>
      <LateStartDate>2012-03-12</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>12</TotalFloat>
      <FreeFloat>12</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>3</OriginalDuration>
      <ID>A0140</ID>
      <Name>Innenputz</Name>
      <EarlyStartDate>2012-03-19</EarlyStartDate>
      <LateStartDate>2012-03-19</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>5</OriginalDuration>
      <ID>A0150</ID>
      <Name>Wärmedämmung</Name>
      <EarlyStartDate>2012-03-05</EarlyStartDate>
      <LateStartDate>2012-03-06</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>1</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>3</OriginalDuration>
      <ID>A0160</ID>
      <Name>Außenputz</Name>
      <EarlyStartDate>2012-03-12</EarlyStartDate>
      <LateStartDate>2012-03-13</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>1</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>2</OriginalDuration>
      <ID>A0149</ID>
      <Name>Gerüst aufbauen</Name>
      <EarlyStartDate>2012-03-01</EarlyStartDate>
      <LateStartDate>2012-03-02</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>1</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>1</OriginalDuration>
      <ID>A0180</ID>
      <Name>Gerüst abbauen</Name>
      <EarlyStartDate>2012-03-15</EarlyStartDate>
      <LateStartDate>2012-03-16</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>1</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>20</OriginalDuration>
      <ID>A0190</ID>
      <Name>Garten</Name>
      <EarlyStartDate>2012-03-16</EarlyStartDate>
      <LateStartDate>2012-03-19</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>1</TotalFloat>
      <FreeFloat>1</FreeFloat>
    </Activity>
    <Milestone>
      <ID>A0000</ID>
      <Name>Baubeginn</Name>
      <EarlyStartDate>2012-01-02</EarlyStartDate>
      <LateStartDate>2012-01-02</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Milestone>
    <Milestone>
      <ID>A0200</ID>
      <Name>Bauende</Name>
      <EarlyStartDate>2012-04-16</EarlyStartDate>
      <LateStartDate>2012-04-16</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Milestone>
    <Activity>
      <OriginalDuration>2</OriginalDuration>
      <ID>A0141</ID>
      <Name>Malerarbeiten</Name>
      <EarlyStartDate>2012-04-05</EarlyStartDate>
      <LateStartDate>2012-04-05</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>10</OriginalDuration>
      <ID>A0091</ID>
      <Name>Rohinstallation Elektro</Name>
      <EarlyStartDate>2012-03-05</EarlyStartDate>
      <LateStartDate>2012-03-05</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>10</OriginalDuration>
      <ID>A0092</ID>
      <Name>Rohinstallation Heizung</Name>
      <EarlyStartDate>2012-03-05</EarlyStartDate>
      <LateStartDate>2012-03-05</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>5</OriginalDuration>
      <ID>A0142</ID>
      <Name>Elektro Fertiginstallation</Name>
      <EarlyStartDate>2012-04-09</EarlyStartDate>
      <LateStartDate>2012-04-09</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
    <Activity>
      <OriginalDuration>5</OriginalDuration>
      <ID>A0143</ID>
      <Name>Heizung Fertiginstallation</Name>
      <EarlyStartDate>2012-04-09</EarlyStartDate>
      <LateStartDate>2012-04-09</LateStartDate>
      <Calendar>0</Calendar>
      <PercentComplete>0</PercentComplete>
      <TotalFloat>0</TotalFloat>
      <FreeFloat>0</FreeFloat>
    </Activity>
  </Activities>
  <Relationships>
    <Relationship>
      <Activity1>A0010</Activity1>
      <Activity2>A0020</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0020</Activity1>
      <Activity2>A0030</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0030</Activity1>
      <Activity2>A0040</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0040</Activity1>
      <Activity2>A0050</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0050</Activity1>
      <Activity2>A0060</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0060</Activity1>
      <Activity2>A0070</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0070</Activity1>
      <Activity2>A0080</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0080</Activity1>
      <Activity2>A0090</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0100</Activity1>
      <Activity2>A0110</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0110</Activity1>
      <Activity2>A0120</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0070</Activity1>
      <Activity2>A0130</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0130</Activity1>
      <Activity2>A0140</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0149</Activity1>
      <Activity2>A0150</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0150</Activity1>
      <Activity2>A0160</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0160</Activity1>
      <Activity2>A0180</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0100</Activity1>
      <Activity2>A0149</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0180</Activity1>
      <Activity2>A0190</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0000</Activity1>
      <Activity2>A0010</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0190</Activity1>
      <Activity2>A0200</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0120</Activity1>
      <Activity2>A0200</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0140</Activity1>
      <Activity2>A0141</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>10</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0090</Activity1>
      <Activity2>A0091</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0091</Activity1>
      <Activity2>A0092</Activity2>
      <DependencyType>StartStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0091</Activity1>
      <Activity2>A0140</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0092</Activity1>
      <Activity2>A0140</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0080</Activity1>
      <Activity2>A0100</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0141</Activity1>
      <Activity2>A0142</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0141</Activity1>
      <Activity2>A0143</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0142</Activity1>
      <Activity2>A0200</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
    <Relationship>
      <Activity1>A0143</Activity1>
      <Activity2>A0200</Activity2>
      <DependencyType>FinishStart</DependencyType>
      <Lag>0</Lag>
    </Relationship>
  </Relationships>
  <Layouts>
    <Layout>
      <ID>0</ID>
      <Name>Neues Layout</Name>
      <ActivityStandardColor>255|255|255|0</ActivityStandardColor>
      <ActivityCriticalColor>255|255|0|0</ActivityCriticalColor>
      <ActivityDoneColor>255|0|0|255</ActivityDoneColor>
      <MilestoneStandardColor>255|0|0|0</MilestoneStandardColor>
      <MilestoneCriticalColor>255|255|0|0</MilestoneCriticalColor>
      <MilestoneDoneColor>255|0|0|255</MilestoneDoneColor>
      <DataDateColor>255|0|0|255</DataDateColor>
      <BaselineColor>255|211|211|211</BaselineColor>
      <ShowBaseline>False</ShowBaseline>
      <ShowRelationships>True</ShowRelationships>
      <ShowFloat>True</ShowFloat>
      <VisibleColumns>
        <ActivityColumn>
          <Property>ID</Property>
        </ActivityColumn>
        <ActivityColumn>
          <Property>Name</Property>
        </ActivityColumn>
        <ActivityColumn>
          <Property>Name</Property>
        </ActivityColumn>
        <ActivityColumn>
          <Property>Name</Property>
        </ActivityColumn>
        <ActivityColumn>
          <Property>Name</Property>
          <ColumnWidth>161,696666666667</ColumnWidth>
        </ActivityColumn>
        <ActivityColumn>
          <Property>OriginalDuration</Property>
        </ActivityColumn>
        <ActivityColumn>
          <Property>OriginalDuration</Property>
        </ActivityColumn>
        <ActivityColumn>
          <Property>OriginalDuration</Property>
        </ActivityColumn>
        <ActivityColumn>
          <Property>OriginalDuration</Property>
          <ColumnWidth>63,9366666666666</ColumnWidth>
        </ActivityColumn>
        <ActivityColumn>
          <Property>StartDate</Property>
        </ActivityColumn>
        <ActivityColumn>
          <Property>FinishDate</Property>
        </ActivityColumn>
        <ActivityColumn>
          <Property>TotalFloat</Property>
        </ActivityColumn>
      </VisibleColumns>
      <SortingDefinitions>
        <SortingDefinition>
          <Property>ID</Property>
          <Direction>Ascending</Direction>
        </SortingDefinition>
      </SortingDefinitions>
      <GroupingDefinitions />
      <LeftText>None</LeftText>
      <CenterText>None</CenterText>
      <RightText>None</RightText>
      <FilterCombination>And</FilterCombination>
      <Filters />
      <VisibleResources />
    </Layout>
  </Layouts>
  <CurrentLayout>0</CurrentLayout>
</Project>