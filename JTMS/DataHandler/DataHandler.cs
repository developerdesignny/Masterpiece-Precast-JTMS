using JTMS.Data;
using JTMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows.Documents;

namespace JTMS.DataHandler
{
    public class DataHandler
    {
        public JTMSContext context;

        public DataHandler()
        {
            context = new JTMSContext();
        }

        public async Task<List<ProjectModel>> GetProjects(int index, string sParam, Guid? companyID = null)
        {
            try
            {
                var projects = new List<ProjectModel>();

                if (companyID != null)
                    projects = await context.Projects.Include(obj => obj.Company).Where(c => c.CompanyId == companyID).
                                                     OrderBy(obj => obj.Company.Id).ToListAsync();
                else
                    projects = await context.Projects.Include(obj => obj.Company).OrderBy(obj => obj.Company.Id).ToListAsync();

                if (!string.IsNullOrEmpty(sParam))
                    projects = projects.Where(obj => obj.JobName.ToLower().Contains(sParam.ToLower().Trim())).ToList();
                return projects;
            }
            catch (Exception) { throw; }
        }//

        public async Task<List<MoldModel>> GetMolds(int index, Guid projectId)
        {
            try
            {
                var molds = await context.Molds.Include(obj => obj.Project).Where(obj => obj.Project.Id == projectId).ToListAsync();
                return molds;
            }
            catch (Exception) { throw; }
        }

        public async Task<List<MoldDetailsModel>> GetSubMolds(MoldModel mold)
        {
            try
            {
                var moldCheck = await context.SubMolds.Where(obj => obj.MoldId == mold.Id).FirstOrDefaultAsync();
                if (moldCheck == null)
                    await AddSubMolds(mold);
                var sMolds = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.MoldId == mold.Id).ToListAsync();
                return sMolds;
            }
            catch (Exception) { throw; }
        }//

        public async Task<List<PalletModel>> GetPallets(int index, Guid projectId)
        {
            try
            {
                var pallets = await context.Pallets.Include(obj => obj.Project).Where(obj => obj.Project.Id == projectId).ToListAsync();
                return pallets;
            }
            catch (Exception) { throw; }
        }//

        public async Task<List<ProgressReportModel>> GetProjectsReports(int index, Guid projectId)
        {
            try
            {
                var reports = await context.ProgressReports.Include(obj => obj.Project).Where(obj => obj.Project.Id == projectId).OrderBy(obj => obj.MoldCode).ToListAsync();
                return reports;
            }
            catch (Exception) { throw; }
        }//

        public async Task<List<ProgressReportModel>> GetProjectsReportsByDate(DateOnly date)
        {
            try
            {
                var reports = await context.ProgressReports.Include(obj => obj.Project).Where(obj => obj.ProcessDate == date).OrderBy(obj => obj.MoldCode).
                                                            OrderBy(obj => obj.ProcessDate).OrderBy(obj => obj.Project.JobName).ToListAsync();
                return reports;
            }
            catch (Exception) { throw; }
        }//

        public async Task<List<PalletModel>> GetFilledPallets(int index, Guid projectId)
        {
            try
            {
                var pallets = await context.Pallets.Include(obj => obj.Project).Where(obj => obj.Project.Id == projectId && obj.IsFilled == true).ToListAsync();
                return pallets;
            }
            catch (Exception) { throw; }
        }//

        public async Task<List<CompanyModel>> GetCompanys(int index)
        {
            try
            {
                var companies = await context.Companies.ToListAsync();
                return companies;
            }
            catch (Exception) { throw; }
        }//


        //...........

        public async Task<bool> DeleteCompany(Guid id)
        {
            try
            {
                var model = await context.Companies.FirstOrDefaultAsync(obj => obj.Id == id);
                if (model != null)
                {
                    context.Companies.Remove(model);
                    await context.SaveChangesAsync();
                    return true;
                }
                else throw new ArgumentNullException("Data not found");
            }
            catch (Exception) { throw; }
        }//

        public async Task<bool> DeleteProject(Guid id)
        {
            try
            {
                var model = await context.Projects.FirstOrDefaultAsync(obj => obj.Id == id);
                if (model != null)
                {
                    context.Projects.Remove(model);
                    await context.SaveChangesAsync();
                    return true;
                }
                else throw new ArgumentNullException("Data not found");
            }
            catch (Exception) { throw; }
        }//

        public async Task<bool> DeleteMold(Guid id)
        {
            try
            {
                var model = await context.Molds.FirstOrDefaultAsync(obj => obj.Id == id);
                if (model != null)
                {
                    context.Molds.Remove(model);
                    await context.SaveChangesAsync();

                    var project = await context.Projects.FirstOrDefaultAsync(obj => obj.Id == model.ProjectId);
                    var Molds = context.Molds.Where(obj => obj.ProjectId == project.Id);
                    var count = Molds.Sum(obj => obj.MoldAmount);
                    var qty = Molds.Sum(obj => obj.ProcessCompleteCount);

                    if (count != 0 && qty != 0)
                        project.PercentageCleared = Math.Round((((double)qty / (double)count) * 100.0), 2);
                    else project.PercentageCleared = 0;

                    project.ProjectUpdateDate = DateTime.Now;
                    await context.SaveChangesAsync();

                    return true;
                }
                else throw new ArgumentNullException("Data not found");
            }
            catch (Exception) { throw; }
        }//

        public async Task<bool> DeleteSubMold(MoldDetailsModel subMold, Guid projectId)
        {
            try
            {
                var mold = context.Molds.FirstOrDefault(obj => subMold.MoldId == obj.Id);
                var pallet = context.Pallets.FirstOrDefault(obj => obj.Id.ToString() == subMold.PalletID.ToString());
                if (pallet != null && mold != null)
                {
                    var codes = pallet.MoldCodes.Split(",").ToList();
                    var c = codes.FirstOrDefault(obj => obj == mold.MoldCode);
                    if (c != null)
                    {
                        codes.Remove(c);
                        pallet.MoldCodes = string.Join(",", codes);
                    }

                    var ids = pallet.MoldIds.Split(",").ToList();
                    var id = ids.FirstOrDefault(obj => obj == subMold.Id.ToString());
                    if (id != null)
                    {
                        ids.Remove(id);
                        pallet.MoldIds = string.Join(",", ids);
                    }
                }
                if (mold != null)
                {
                    mold.MoldAmount -= 1;
                    if (mold.MoldAmount == 0)
                        context.Molds.Remove(mold);
                }
                var sMold = context.SubMolds.FirstOrDefault(obj => obj.Id == subMold.Id);
                if (sMold != null)
                {
                    context.SubMolds.Remove(sMold);
                    await context.SaveChangesAsync();

                    if (mold != null)
                    {
                        if (mold.MoldAmount > 0)
                        {
                            mold.Process1Count = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process1Complete == true && obj.MoldId == mold.Id).CountAsync();
                            mold.Process2Count = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process2Complete == true && obj.MoldId == mold.Id).CountAsync();
                            mold.Process3Count = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process3Complete == true && obj.MoldId == mold.Id).CountAsync();
                            mold.Process4Count = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process4Complete == true && obj.MoldId == mold.Id).CountAsync();
                            mold.ProcessCompleteCount = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process1Complete == true &&
                                                                                        obj.Process3Complete == true && obj.Process4Complete == true && obj.MoldId == mold.Id).CountAsync();
                        }
                        var project = await context.Projects.FirstOrDefaultAsync(obj => obj.Id == mold.ProjectId);
                        await context.SaveChangesAsync();

                        var Molds = context.Molds.Where(obj => obj.ProjectId == project.Id);
                        var count = Molds.Sum(obj => obj.MoldAmount);
                        var qty = Molds.Sum(obj => obj.ProcessCompleteCount);

                        if (count != 0 && qty != 0)
                            project.PercentageCleared = Math.Round((((double)qty / (double)count) * 100.0), 2);
                        else project.PercentageCleared = 0;

                        project.ProjectUpdateDate = DateTime.Now;
                        await context.SaveChangesAsync();
                    }
                    return true;
                }
                else throw new ArgumentNullException("Data not found");
            }
            catch (Exception) { throw; }
        }//

        public async Task<bool> DeleteProgressReports(Guid id)
        {
            try
            {
                var model = await context.ProgressReports.FirstOrDefaultAsync(obj => obj.Id == id);
                if (model != null)
                {
                    context.ProgressReports.Remove(model);
                    await context.SaveChangesAsync();
                    return true;
                }
                else throw new ArgumentNullException("Data not found");
            }
            catch (Exception) { throw; }
        }//

        //...........

        public async Task<bool> AddCompany(CompanyModel company)
        {
            try
            {
                await context.Companies.AddAsync(company);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception) { throw; }
        }//

        public async Task<bool> AddProject(ProjectModel project)
        {
            try
            {
                await context.Projects.AddAsync(project);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception) { throw; }
        }//

        public async Task<bool> AddMold(MoldModel mold)
        {
            try
            {
                await context.Molds.AddAsync(mold);
                await context.SaveChangesAsync();
                _ = Task.Run(async () =>
                  {
                      await AddSubMolds(mold);
                  });

                var project = await context.Projects.FirstOrDefaultAsync(obj => obj.Id == mold.ProjectId);
                var Molds = context.Molds.Where(obj => obj.ProjectId == project.Id);
                var count = Molds.Sum(obj => obj.MoldAmount);
                var qty = Molds.Sum(obj => obj.ProcessCompleteCount);

                if (count != 0 && qty != 0)
                    project.PercentageCleared = Math.Round((((double)qty / (double)count) * 100.0), 2);
                else project.PercentageCleared = 0;

                project.ProjectUpdateDate = DateTime.Now;
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception) { throw; }
        }//

        public async Task AddSubMolds(MoldModel mold)
        {
            for (int i = 0; i < mold.MoldAmount; ++i)
            {
                var sMold = new MoldDetailsModel();
                sMold.Id = new Guid();
                sMold.MoldId = mold.Id;
                sMold.QIndex = i + 1;
                await context.SubMolds.AddAsync(sMold);
            }
            await context.SaveChangesAsync();
        }

        public async Task AddSubMolds(MoldModel mold, int amount)
        {
            var subModel = context.SubMolds.Where(obj => obj.MoldId == mold.Id).OrderByDescending(obj => obj.QIndex).FirstOrDefault();
            var index = 1;
            if (subModel != null)
                index = subModel.QIndex + 1;

            for (int i = 0; i < amount; ++i)
            {
                var sMold = new MoldDetailsModel();
                sMold.Id = new Guid();
                sMold.MoldId = mold.Id;
                sMold.QIndex = index;
                ++index;
                await context.SubMolds.AddAsync(sMold);
            }

            var dmold = context.Molds.FirstOrDefault(obj => mold.Id == obj.Id);
            if (dmold != null)
                dmold.MoldAmount += amount;

            var project = await context.Projects.FirstOrDefaultAsync(obj => obj.Id == mold.ProjectId);
            var Molds = context.Molds.Where(obj => obj.ProjectId == project.Id);
            var count = Molds.Sum(obj => obj.MoldAmount);
            var qty = Molds.Sum(obj => obj.ProcessCompleteCount);
            if (count != 0 && qty != 0)
                project.PercentageCleared = Math.Round((((double)qty / (double)count) * 100.0), 2);
            else project.PercentageCleared = 0;

            await context.SaveChangesAsync();
        }

        public async Task<bool> AddMoldRange(List<MoldModel> molds)
        {
            try
            {
                await context.Molds.AddRangeAsync(molds);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception) { throw; }
        }//

        public async Task<bool> AddProgressReport(ProgressReportModel progressReport)
        {
            try
            {
                await context.ProgressReports.AddAsync(progressReport);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception) { throw; }
        }//

        public async Task<bool> AddPallet(PalletModel model)
        {
            try
            {
                await context.Pallets.AddAsync(model);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception) { throw; }
        }//


        //...........

        public async Task<bool> EditCompany(CompanyModel company)
        {
            try
            {
                var model = await context.Companies.FirstOrDefaultAsync(obj => obj.Id == company.Id);
                if (model != null)
                {
                    model.Name = company.Name;
                    await context.SaveChangesAsync();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception) { throw; }
        }//

        public async Task<bool> EditProject(ProjectModel project)
        {
            try
            {
                var model = await context.Projects.Include(obj => obj.Company).FirstOrDefaultAsync(obj => obj.Id == project.Id);
                if (model != null)
                {
                    model.JobName = project.JobName;
                    model.JobDate = project.JobDate;
                    model.JobColor = project.JobColor;
                    model.CompanyId = project.Company.Id;
                    model.Company = project.Company;
                    model.Address = project.Address;
                    await context.SaveChangesAsync();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception) { throw; }
        }//

        public async Task<bool> EditMold(MoldModel mold)
        {
            try
            {
                var model = await context.Molds.FirstOrDefaultAsync(obj => obj.Id == mold.Id);
                if (model != null)
                {
                    model.MoldAmount = mold.MoldAmount;
                    model.MoldSize = mold.MoldSize;
                    model.MoldCode = mold.MoldCode;
                    model.MoldCut = mold.MoldCut;
                    model.MoldPhoto = mold.MoldPhoto;
                    model.MoldInfo = mold.MoldInfo;
                    model.Cut = mold.Cut;
                    model.Paint = mold.Paint;
                    await context.SaveChangesAsync();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception) { throw; }
        }//

        public async Task UpdateProject(MoldDetailsModel mold, bool removeA = false)
        {
            var model = await context.SubMolds.FirstOrDefaultAsync(obj => obj.Id == mold.Id);
            model.Process1Complete = mold.Process1Complete;
            model.Process2Complete = mold.Process2Complete;
            model.Process3Complete = mold.Process3Complete;
            model.Process4Complete = mold.Process4Complete;
            model.PalletID = mold.PalletID;
            var mMold = await context.Molds.FirstOrDefaultAsync(obj => obj.Id == mold.Mold.Id);
            if (mMold != null && removeA == true)
                mMold.MoldAmount -= 1;
            mMold.Process1Count = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process1Complete == true && obj.MoldId == mMold.Id).CountAsync();
            mMold.Process2Count = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process2Complete == true && obj.MoldId == mMold.Id).CountAsync();
            mMold.Process3Count = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process3Complete == true && obj.MoldId == mMold.Id).CountAsync();
            mMold.Process4Count = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process4Complete == true && obj.MoldId == mMold.Id).CountAsync();
            mMold.ProcessCompleteCount = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process1Complete == true &&
                                                                        obj.Process3Complete == true && obj.Process4Complete == true && obj.MoldId == mMold.Id).CountAsync();
            var project = await context.Projects.FirstOrDefaultAsync(obj => obj.Id == mold.Mold.ProjectId);
            await context.SaveChangesAsync();

            var Molds = context.Molds.Where(obj => obj.ProjectId == project.Id);
            var count = Molds.Sum(obj => obj.MoldAmount);
            var qty = Molds.Sum(obj => obj.ProcessCompleteCount);

            if (count != 0 && qty != 0)
                project.PercentageCleared = Math.Round((((double)qty / (double)count) * 100.0), 2);
            else project.PercentageCleared = 0;

            project.ProjectUpdateDate = DateTime.Now;
            await context.SaveChangesAsync();
        }

        public async Task<bool> UpdateMoldProcess(MoldDetailsModel mold)
        {
            try
            {
                var model = await context.SubMolds.FirstOrDefaultAsync(obj => obj.Id == mold.Id);
                if (model != null)
                {
                    if (mold.Process4Complete == true)
                    {
                        var pallets = await context.Pallets.Where(obj => obj.ProjectId == mold.Mold.ProjectId).OrderBy(obj => obj.Name).ToListAsync();
                        var lastPallet = pallets.LastOrDefault();
                        var newName = "Pallet-1";

                        if (lastPallet != null)
                        {
                            var spilt = lastPallet.Name.Split("-")[1];
                            newName = "Pallet-" + (int.Parse(spilt) + 1).ToString();
                        }

                        var pallet = await context.Pallets.FirstOrDefaultAsync(obj => obj.ProjectId == mold.Mold.ProjectId && obj.IsFilled == false);
                        if (pallet == null)
                        {
                            pallet = new PalletModel
                            {
                                ProjectId = mold.Mold.ProjectId,
                                Name = newName,
                                MoldCodes = $"{mold.Mold.MoldCode}",
                                MoldIds = $"{mold.Id}"

                            };
                            await context.Pallets.AddAsync(pallet);
                            await context.SaveChangesAsync();
                            pallet = await context.Pallets.FirstOrDefaultAsync(obj => obj.ProjectId == mold.Mold.ProjectId && obj.IsFilled == false);
                        }
                        else
                        {
                            pallet.MoldCodes += $",{mold.Mold.MoldCode}";
                            pallet.MoldIds += $",{mold.Id}";
                        }

                        mold.PalletID = pallet.Id.ToString();
                    }
                    model.Process1Complete = mold.Process1Complete;
                    model.Process2Complete = mold.Process2Complete;
                    model.Process3Complete = mold.Process3Complete;
                    model.Process4Complete = mold.Process4Complete;
                    var mMold = await context.Molds.FirstOrDefaultAsync(obj => obj.Id == mold.Mold.Id);

                    mMold.Process1Count = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process1Complete == true && obj.MoldId == mMold.Id).CountAsync();
                    mMold.Process2Count = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process2Complete == true && obj.MoldId == mMold.Id).CountAsync();
                    mMold.Process3Count = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process3Complete == true && obj.MoldId == mMold.Id).CountAsync();
                    mMold.Process4Count = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process4Complete == true && obj.MoldId == mMold.Id).CountAsync();
                    mMold.ProcessCompleteCount = await context.SubMolds.Include(obj => obj.Mold).Where(obj => obj.Process1Complete == true &&
                                                                                obj.Process3Complete == true && obj.Process4Complete == true && obj.MoldId == mMold.Id).CountAsync();
                    var project = await context.Projects.FirstOrDefaultAsync(obj => obj.Id == mold.Mold.ProjectId);
                    await context.SaveChangesAsync();

                    var Molds = context.Molds.Where(obj => obj.ProjectId == project.Id);
                    var count = Molds.Sum(obj => obj.MoldAmount);
                    var qty = Molds.Sum(obj => obj.ProcessCompleteCount);

                    if (count != 0 && qty != 0)
                        project.PercentageCleared = Math.Round((((double)qty / (double)count) * 100.0), 2);
                    else project.PercentageCleared = 0;

                    project.ProjectUpdateDate = DateTime.Now;
                    await context.SaveChangesAsync();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception) { throw; }
        }//

        public async Task<bool> UpdatePallet(Guid projectID)
        {
            var pallet = await context.Pallets.FirstOrDefaultAsync(obj => obj.ProjectId == projectID && obj.IsFilled == false);
            if (pallet == null)
                return false;
            else
            {
                pallet.IsFilled = true;
                await context.SaveChangesAsync();
                return true;
            }
        }//

    }
}
